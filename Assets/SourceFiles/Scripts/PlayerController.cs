using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotationSpeed = 120.0f;
    [SerializeField] private float jumpVelocity = 8.0f;
    [SerializeField] private float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private bool jumpRequested;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        var playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];

            jumpAction.performed += _ => jumpRequested = true;
        }
        else
        {
            Debug.LogWarning("PlayerController: No PlayerInput component found. " +
                             "Add one and assign your PlayerInputs asset.");
        }
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        jumpAction.performed -= _ => jumpRequested = true;
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        Vector3 movement = transform.forward * moveInput.y * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (jumpRequested && IsGrounded())
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = jumpVelocity;
            rb.linearVelocity = velocity;
        }
        jumpRequested = false;

        float turnDirection = moveInput.x;

        if (moveInput.y < 0f)
            turnDirection = -turnDirection;

        float turn = turnDirection * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}