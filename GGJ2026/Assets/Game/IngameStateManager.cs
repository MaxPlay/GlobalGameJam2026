using System.Collections.Generic;
using UnityEngine;

public class IngameStateManager : GameStateManager
{
    private readonly List<Person> people = new();
    public IReadOnlyList<Person> People => people;

    public override Game.GameState State => Game.GameState.Ingame;

    public void Start()
    {
        people.AddRange(FindObjectsByType<Person>(FindObjectsSortMode.None));
    }
}
