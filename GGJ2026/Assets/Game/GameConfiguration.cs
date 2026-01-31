using Alchemy.Inspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{
    [SerializeField, BoxGroup("Scenes")]
    private string splash;
    [SerializeField, BoxGroup("Scenes")]
    private string menu;
    [SerializeField, BoxGroup("Scenes")]
    private string ingame;
    [SerializeField, BoxGroup("Scenes")]
    private string options;
    [SerializeField, BoxGroup("Scenes")]
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
