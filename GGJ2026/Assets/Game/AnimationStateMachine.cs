using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationStateMachine<T> where T : Enum
{
    private T currentState;

    private Dictionary<T, string> stateMapping;

    [SerializeField] private Animatable animations;

    public bool locked;

    public void Setup(T defaultState, params (T, string)[] values)
    {
        stateMapping = new Dictionary<T, string>();
        foreach ((T, string) value in values)
        {
            stateMapping[value.Item1] = value.Item2;
        }

        TryPlayState(defaultState);
    }

    public bool TryPlayState(T newState, int delay = 0, Action onEnd = null, bool locke = false, params Animatable.AnimationEvent[] events)
    {
        if (!stateMapping.ContainsKey(newState) || currentState.Equals(newState) || locked)
            return false;

        animations.PlayAnimation(stateMapping[newState], delay, onEnd, events);
        currentState = newState;

        locked = locke;

        return true;
    }
}
