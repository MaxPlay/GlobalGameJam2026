using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameStateManager : GameStateManager
{
    public enum IngameState
    {
        Running,
        Paused,
        Over
    }

    private int currentDay = 1;
    private Player player;

    public Player Player => player;
    public IngameState State { get; private set; }

    private readonly List<RefillStation> refillStations = new();
    private readonly List<Person> people = new();
    public IReadOnlyList<Person> People => people;

    public override Game.GameState RepresentsState => Game.GameState.Ingame;

    [SerializeField]
    private IngameHud hudPrefab;
    private IngameHud hudInstance;
    private InputAction peopleListAction;

    [SerializeField, BoxGroup("Ending")] private Transform cinematicStart;
    [SerializeField, BoxGroup("Ending")] private Transform cinematicEnd;
    [SerializeField, BoxGroup("Ending")] private float cinematicDuration = 30;

    public void Start()
    {
        people.AddRange(FindObjectsByType<Person>(FindObjectsSortMode.None));
        refillStations.AddRange(FindObjectsByType<RefillStation>(FindObjectsSortMode.None));
        player = FindFirstObjectByType<Player>();
        hudInstance = Game.Instance.AddUI(hudPrefab);
        hudInstance.Setup(this);
        Game.Instance.EnableInput();
        StartCoroutine(NextDayRoutine(true));

        peopleListAction = InputSystem.actions.FindAction("PeopleList");
    }

    private void Update()
    {
        if (State != IngameState.Over && peopleListAction.WasPressedThisFrame())
            TogglePeopleList();
    }

    public void OnDestroy()
    {
        Destroy(hudInstance.gameObject);
        Game.Instance.DisableInput();
    }

    public void GameLost()
    {
        State = IngameState.Over;
        StartCoroutine(StartGameLossSequence());
    }

    private IEnumerator StartGameLossSequence()
    {
        yield return new WaitForSeconds(2);
        hudInstance.ShowLossOverlay();
    }

    public void GameWon()
    {
        State = IngameState.Over;
        StartCoroutine(StartGameWinSequence());
    }

    private IEnumerator StartGameWinSequence()
    {
        yield return new WaitForSeconds(2);
        Camera.main.transform.DOMove(cinematicEnd.position, cinematicDuration).From(cinematicStart.position).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        Camera.main.transform.rotation = cinematicStart.rotation;
        hudInstance.ShowWinOverlay();
        hudInstance.GameWin.Setup(this, currentDay, people.Count(p => p.State == Person.PersonState.Alive), people.Count(p => p.State == Person.PersonState.Dead));

    }

    public void EndGame()
    {
        Game.Instance.SwitchState(Game.GameState.Menu);
    }

    public void ValidatePeople()
    {
        if (people.TrueForAll(p => p.IsRecorded))
        {
            GameWon();
        }
    }

    public void NextDay()
    {
        StartCoroutine(NextDayRoutine());
    }

    public IEnumerator NextDayRoutine(bool firstDay = false)
    {
        Game.Instance.DisableInput();

        if (!firstDay)
        {
            ++currentDay;
        }

        int deadCount = people.Count(p => p.State == Person.PersonState.Dead);
        hudInstance.NextDay.SetData(currentDay, deadCount, people.Count);
        yield return hudInstance.NextDay.Show(firstDay);

        player.RefillMask(Mathf.CeilToInt(player.TotalMaskPoints));

        foreach (RefillStation item in refillStations)
        {
            item.NextDay();
        }

        foreach (Person item in people)
        {
            item.NextDay();
        }

        yield return new WaitForSeconds(3);
        yield return hudInstance.NextDay.Hide();

        Game.Instance.EnableInput();
    }

    public void TogglePeopleList(bool force = false)
    {
        if (force || State != IngameState.Over)
            State = hudInstance.TogglePeopleList() ? IngameState.Paused : IngameState.Running;
    }
}
