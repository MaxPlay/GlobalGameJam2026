using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction usePouchAction;

    private CharacterController characterController;
    private Player player;
    private IngameStateManager gameState;

    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private float sprintSpeed;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        interactAction = InputSystem.actions.FindAction("Interact");
        usePouchAction = InputSystem.actions.FindAction("UsePouch");

        characterController = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        gameState = Game.Instance.GetStateManager<IngameStateManager>();
    }

    private void Update()
    {
        if (gameState.State != IngameStateManager.IngameState.Running)
            return;

        bool sprinting = sprintAction.IsPressed();
        Vector3 position = transform.position;
        float totalSpeed = sprinting ? sprintSpeed : speed;
        Vector2 moveValue = moveAction.ReadValue<Vector2>() * totalSpeed;

        characterController.SimpleMove(new Vector3(moveValue.x, 0, moveValue.y));
        player.MoveDistance(Vector3.Distance(transform.position, position), sprinting, moveValue);

        if (interactAction.WasPressedThisFrame())
            player.Interact();

        if (usePouchAction.WasPressedThisFrame())
            player.UsePouch();
    }
}
