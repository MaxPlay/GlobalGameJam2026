using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private GameStateManager currentStateManager;
    private InputActionMap playerActionMap;

    public T GetStateManager<T>() where T : GameStateManager => currentStateManager as T;

    public static Game Instance { get; private set; }
    public GameConfiguration Configuration { get; private set; }

    public GameState CurrentState { get; private set; }

    public enum GameState
    {
        Splash,
        Menu,
        Ingame,
        Options,
        Credits
    }

    [RuntimeInitializeOnLoadMethod]
    public static void Main()
    {
        InputSystem.actions.Enable();

        GameConfiguration configuration = Resources.Load<GameConfiguration>("Configuration");
        Debug.Assert(configuration, "Could not find file Configuration in Resources");

        Game game = FindFirstObjectByType<Game>();
        if (!game)
        {
            GameObject mainObject = new GameObject("Game");
            game = mainObject.AddComponent<Game>();
        }
        DontDestroyOnLoad(game.gameObject);
        game.Startup(configuration);
    }

    private void Startup(GameConfiguration configuration)
    {
        Instance = this;
        Configuration = configuration;
        playerActionMap = InputSystem.actions.FindActionMap("Player");
        DisableInput();
    }

    public void Start()
    {
        GameState? state = DetectState();
        if (!state.HasValue)
        {
            SceneManager.LoadScene(Configuration.GetSceneByState(GameState.Splash));
            state = DetectState();
            if (!state.HasValue)
            {
                Debug.LogError("Splash Screen has no state manager");
                return;
            }
        }
        CurrentState = state.Value;
    }

    private GameState? DetectState()
    {
        if (!currentStateManager)
        {
            currentStateManager = FindFirstObjectByType<GameStateManager>();
            if (currentStateManager)
                return currentStateManager.RepresentsState;
            return null;
        }
        return currentStateManager.RepresentsState;
    }

    private void Update()
    {
    }

    public void SwitchState(GameState state)
    {
        if (CurrentState != state)
            return;

        CurrentState = state;
        string scene = Configuration.GetSceneByState(CurrentState);
        SceneManager.LoadScene(scene);
        currentStateManager = FindFirstObjectByType<GameStateManager>();
        Debug.Assert(currentStateManager, "State Scene has no state manager");
        Debug.Assert(currentStateManager.RepresentsState != CurrentState, "Current State Manager state not matching to desired state");
    }

    public void DisableInput()
    {
        playerActionMap.Disable();
    }

    public void EnableInput()
    {
        playerActionMap.Enable();
    }
}

public abstract class GameStateManager : MonoBehaviour
{
    public abstract Game.GameState RepresentsState { get; }
}
