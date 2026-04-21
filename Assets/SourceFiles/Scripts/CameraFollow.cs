using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;           // Drag Player here in Inspector
    public float distance = 5f;
    public float minDistance = 1.5f;
    public float maxDistance = 10f;

    [Header("Camera Look (Mouse / Right Stick)")]
    public float mouseSensitivity = 2f;
    public float verticalMin = -20f;   // How far down camera can tilt
    public float verticalMax = 60f;    // How far up camera can tilt

    [Header("Smoothing")]
    public float positionSmoothing = 12f;

    // Internal state
    private float yaw;    // Horizontal orbit angle (left/right)
    private float pitch;  // Vertical orbit angle (up/down)

    // Input Action for camera look — same action works for Mouse AND PS5 Right Stick
    private InputAction lookAction;

    private void Awake()
    {
        // Resolve the Look action from the PlayerInput on the Player object.
        // If your camera is on the same GameObject as PlayerInput, use GetComponent directly.
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
            lookAction = playerInput.actions["Look"];

        // Lock cursor for mouse look (comment out if you don't want this)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() { lookAction?.Enable(); }
    private void OnDisable() { lookAction?.Disable(); }

    private void LateUpdate()
    {
        if (target == null) return;

        // ── 1. READ LOOK INPUT ────────────────────────────────────────────────
        // lookAction fires from Mouse/delta (keyboard scheme) OR
        // Gamepad/rightStick (PS5 DualSense) — same action, no device check needed.
        Vector2 look = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;

        yaw += look.x * mouseSensitivity;
        pitch -= look.y * mouseSensitivity;   // subtract = moving mouse up tilts up
        pitch = Mathf.Clamp(pitch, verticalMin, verticalMax);

        // ── 2. CALCULATE DESIRED CAMERA POSITION ─────────────────────────────
        // Clamp distance to safe range so camera can never clip at extremes
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Orbit the camera around the target using yaw + pitch angles
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 orbitOffset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = target.position + orbitOffset;

        // Direction from player pivot outward toward desired camera position
        Vector3 dir = (desiredPos - target.position).normalized;

        // ── 3. SPHERE CAST COLLISION DETECTION ───────────────────────────────
        // Fire a SphereCast from the player pivot toward the desired camera position.
        // SphereCast (vs Raycast) is used because it has volume — it catches cases
        // where the camera partially clips a corner that a thin ray would miss.
        //
        // Collision scenarios handled:
        //   Terrain  → SphereCast hits the ground Box Collider, camera stays above
        //   Walls    → Camera is pulled to just in front of the wall surface
        //   Objects  → Barrels, stairs, boxes all have colliders — treated like walls
        float finalDist = distance;
        float sphereRadius = 0.2f;

        if (Physics.SphereCast(target.position, sphereRadius, dir,
            out RaycastHit hit, distance))
        {
            // Pull camera to just in front of the hit surface (0.2f offset avoids z-fighting)
            finalDist = Mathf.Clamp(hit.distance - 0.2f, minDistance, distance);
        }

        // ── 4. APPLY POSITION (smoothed to avoid snapping) ───────────────────
        Vector3 targetPos = target.position + dir * finalDist;
        transform.position = Vector3.Lerp(transform.position, targetPos,
                                          positionSmoothing * Time.deltaTime);

        // Always look at the player's approximate centre of mass
        transform.LookAt(target.position + Vector3.up * 1.0f);
    }
}