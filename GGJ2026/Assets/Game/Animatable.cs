using System;
using System.Collections;
using UnityEngine;

public class Animatable : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string state, int delay = 0, Action onEnd = null, params AnimationEvent[] events)
    {
        StopAllCoroutines();
        if (delay > 0)
        {
            StartCoroutine(DelayAnimation(state, delay, onEnd, events));
            return;
        }

        StartCoroutine(DoPlayAnimation(state, onEnd, events));
    }

    private IEnumerator DelayAnimation(string state, int delay, Action onEnd = null, params AnimationEvent[] events)
    {
        yield return new WaitForSeconds(delay / 30f);
        StartCoroutine(DoPlayAnimation(state, onEnd, events));
    }

    private IEnumerator DoPlayAnimation(string state, Action onEnd, AnimationEvent[] events)
    {
        animator.Play(state);
        yield return null; // animators don't update their "current state" immediately. Wait 1 Frame to ensure we get the correct clip length;

        foreach (AnimationEvent actionEvent in events)
        {
            actionEvent.callTime = actionEvent.callTime - 1;
            StartCoroutine(PrepareEvent(actionEvent));
        }

        if (onEnd != null)
        {
            float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(PrepareEvent(new AnimationEvent()
            {
                callTime = Mathf.RoundToInt(clipLength * 30 - 1),
                action = onEnd
            }));
        }
    }

    private IEnumerator PrepareEvent(AnimationEvent actionEvent)
    {
        yield return new WaitForSeconds(actionEvent.callTime / 30f);
        actionEvent.action?.Invoke();
    }

    public class AnimationEvent
    {
        public Action action;
        public int callTime;
    }
}
