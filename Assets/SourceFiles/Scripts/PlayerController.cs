using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves forward/backward and rotates with WASD/Arrow keys OR PS5 DualSense left stick.
/// Uses the New Input System (action-based) so the same actions fire from both devices.
///
/// SETUP REQUIRED (see guide):
///   1. Add a PlayerInput component to this GameObject
///   2. Assign your PlayerInputs.inputactions asset to it
///   3. The asset must have a "Player" action map with:
///      - "Move"  (Value, Vector2) — WASD bound + Gamepad/leftStick
///      - "Look"  (Value, Vector2) — Mouse/delta bound + Gamepad/rightStick
///      - "Jump"  (Button)         — Space bound + Gamepad/buttonSouth (Cross ×)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rotationSpeed = 120.0f;
    [SerializeField] private float jumpVelocity = 8.0f;
    [SerializeField] private float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private bool jumpRequested;

    // ── Input Action references ──────────────────────────────────────────────
    // These are resolved from the PlayerInput component at startup.
    // The same action works for Keyboard/Mouse AND PS5 DualSense — no if/else.
    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Grab the PlayerInput component that holds the .inputactions asset
        var playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            // Bind to named actions — device-agnostic (keyboard OR PS5, whichever fires)
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];

            // Subscribe to jump as an event so we never miss a quick tap
            jumpAction.performed += _ => jumpRequested = true;
        }
        else
        {
            // Fallback warning — PlayerInput component is required
            Debug.LogWarning("PlayerController: No PlayerInput component found. " +
                             "Add one and assign your PlayerInputs asset.");
        }
    }

    private void OnEnable()
    {
        // Enable actions when this object is active
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        // Always disable and unsubscribe to avoid memory leaks
        jumpAction.performed -= _ => jumpRequested = true;
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    private void FixedUpdate()
    {
        // ReadValue works identically whether input came from WASD or PS5 Left Stick
        // Keyboard composite gives -1/0/+1; gamepad stick gives a smooth float
        Vector2 moveInput = moveAction != null
            ? moveAction.ReadValue<Vector2>()
            : Vector2.zero;

        // ── Movement (same logic as before, just input source changed) ───────
        // Move in the direction the player is facing (forward/backward)
        Vector3 movement = transform.forward * moveInput.y * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // ── Jump ─────────────────────────────────────────────────────────────
        // jumpRequested is set by the action event (keyboard Space OR Cross ×)
        if (jumpRequested && IsGrounded())
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = jumpVelocity;
            rb.linearVelocity = velocity;
        }
        jumpRequested = false;

        // ── Y-axis rotation ───────────────────────────────────────────────────
        // moveInput.x is negative=left, positive=right (WASD A/D or stick X axis)
        float turnDirection = moveInput.x;

        // Invert turn when walking backward (same as original behaviour)
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