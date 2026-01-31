using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    public static void Main()
    {
        InputSystem.actions.Enable();

        Game game = FindFirstObjectByType<Game>();
        if (!game)
        {
            GameObject mainObject = new GameObject("Game");
            game = mainObject.AddComponent<Game>();
        }
        DontDestroyOnLoad(game.gameObject);
        game.Startup();
    }

    private void Startup()
    {

    }
}