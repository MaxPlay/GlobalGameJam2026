using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PanelBehaviour : MonoBehaviour
{
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] private StatePositionMapping defaultPosition;
    [SerializeField] private List<StatePositionMapping> statePositions;

    public void MoveToState(MainMenuStateManager.MainMenuStates state, bool instantMovement)
    {
        transform.DOKill();
        foreach (StatePositionMapping statePosition in statePositions)
        {
            if (statePosition.state == state)
            {
                statePosition.MoveTo(this, instantMovement);
                return;
            }
        }

        defaultPosition.MoveTo(this, instantMovement);
    }

    [Serializable]
    public class StatePositionMapping
    {
        public MainMenuStateManager.MainMenuStates state;
        public Transform targetPosition;
        public float transitionDuration = 1;
        public bool enable;

        public void MoveTo(PanelBehaviour transformation, bool instantMovement)
        {
            if (!targetPosition)
                return;
            if (instantMovement)
            {
                transformation.transform.position = targetPosition.position;
                if(transformation.canvasGroup)
                    transformation.canvasGroup.interactable = enable;
                return;
            }
            if (transformation.canvasGroup)
                transformation.canvasGroup.interactable = false;
            transformation.transform.DOMove(targetPosition.position, transitionDuration).OnComplete((() =>
            {
                if(enable && transformation.canvasGroup)
                    transformation.canvasGroup.interactable = true;
            }));
        }
    }
}
