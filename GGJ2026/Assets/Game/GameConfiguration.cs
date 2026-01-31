using Alchemy.Inspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{
    [SerializeField, Group("Scenes")]
    private string splash;
    [SerializeField, Group("Scenes")]
    private string menu;
    [SerializeField, Group("Scenes")]
    private string ingame;
    [SerializeField, Group("Scenes")]
    private string options;
    [SerializeField, Group("Scenes")]
    private string credits;

    public string GetSceneByState(Game.GameState state)
    {
        return state switch
        {
            Game.GameState.Splash => splash,
            Game.GameState.Menu => menu,
            Game.GameState.Ingame => ingame,
            Game.GameState.Options => options,
            Game.GameState.Credits => credits,
            _ => throw new System.Exception("Unknown state"),
        };
    }
}
