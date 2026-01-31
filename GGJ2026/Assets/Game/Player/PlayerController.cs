using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction jumpAction;
    private CharacterController characterController;
    [SerializeField]
    private Transform sprite;
    [SerializeField]
    private float speed = 1;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 position = transform.position;
        Vector2 moveValue = moveAction.ReadValue<Vector2>() * speed;

        characterController.SimpleMove(new Vector3(moveValue.x, 0, moveValue.y));

        if (jumpAction.IsPressed())
        {
            sprite.DOLocalJump(Vector3.zero, 2, 1, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
