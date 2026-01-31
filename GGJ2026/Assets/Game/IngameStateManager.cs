using System;
using System.Collections;
using System.Collections.Generic;
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

    public IngameState State { get; private set; }

    private readonly List<RefillStation> refillStations = new();
    private readonly List<Person> people = new();
    public IReadOnlyList<Person> People => people;

    public override Game.GameState RepresentsState => Game.GameState.Ingame;


    public void Start()
    {
        people.AddRange(FindObjectsByType<Person>(FindObjectsSortMode.None));
        refillStations.AddRange(FindObjectsByType<RefillStation>(FindObjectsSortMode.None));
        player = FindFirstObjectByType<Player>();
        Game.Instance.EnableInput();
    }

    public void OnDestroy()
    {
        Game.Instance.DisableInput();
    }

    public void GameLost()
    {
        State = IngameState.Over;
        Debug.Log("Game Lost!");
    }

    public void GameWon()
    {
        State = IngameState.Over;
        Debug.Log("Game Won!");
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

        // TODO: Animate Overlay
        yield return null;

        foreach (RefillStation item in refillStations)
        {
            item.NextDay();
        }

        foreach (Person item in people)
        {
            item.NextDay();
        }

        // TODO: Animate Overlay
        yield return null;

        Game.Instance.EnableInput();
    }
}
