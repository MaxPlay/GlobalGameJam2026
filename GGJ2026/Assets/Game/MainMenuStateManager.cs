using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainMenuStateManager : GameStateManager
{
    public override Game.GameState RepresentsState => Game.GameState.Menu;

    private PanelBehaviour[] panels;

    [SerializeField] private Transform cameraController;
    [SerializeField] private Transform cameraTarget;

    private void Start()
    {
        panels = FindObjectsByType<PanelBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ChangeState(MainMenuStates.Main, true);
    }

    public void PlayGame()
    {
        ChangeState(MainMenuStates.Play);
        cameraController.DOMove(cameraTarget.position, 3f).SetEase(Ease.InOutQuad).OnComplete(StartPlayState);
    }

    private void StartPlayState()
    {
        Game.Instance.SwitchState(Game.GameState.Ingame);
    }

    public void OpenCredits()
    {
        ChangeState(MainMenuStates.Credits);
    }

    public void GoBack()
    {
        ChangeState(MainMenuStates.Main);
    }

    private void ChangeState(MainMenuStates state, bool instantMovement = false)
    {
        foreach (PanelBehaviour panel in panels)
        {
            panel.MoveToState(state, instantMovement);
        }
    }

    public enum MainMenuStates
    {
        Main,
        Credits,
        Play
    }
}
