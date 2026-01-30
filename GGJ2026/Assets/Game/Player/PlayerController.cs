using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction jumpAction;

    [SerializeField]
    private Transform sprite;

    private new Rigidbody rigidbody;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        rigidbody.AddForce(new Vector3(moveValue.x, 0, moveValue.y), ForceMode.Force);

        if (jumpAction.IsPressed())
        {
            sprite.DOLocalJump(Vector3.zero, 2, 1, 1);
        }
    }
}
