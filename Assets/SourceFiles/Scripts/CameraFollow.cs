using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float distance = 5f;
    public float minDistance = 1.5f;
    public float maxDistance = 10f;

    [Header("Camera Look (Mouse / Right Stick)")]
    public float mouseSensitivity = 2f;
    public float verticalMin = -20f;
    public float verticalMax = 60f;

    [Header("Smoothing")]
    public float positionSmoothing = 12f;

    private float yaw;
    private float pitch; 

    private InputAction lookAction;

    private void Awake()
    {
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
            lookAction = playerInput.actions["Look"];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() { lookAction?.Enable(); }
    private void OnDisable() { lookAction?.Disable(); }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 look = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;

        yaw += look.x * mouseSensitivity;
        pitch -= look.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, verticalMin, verticalMax);

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 orbitOffset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = target.position + orbitOffset;

        Vector3 dir = (desiredPos - target.position).normalized;

        float finalDist = distance;
        float sphereRadius = 0.2f;

        if (Physics.SphereCast(target.position, sphereRadius, dir,
            out RaycastHit hit, distance))
        {
            finalDist = Mathf.Clamp(hit.distance - 0.2f, minDistance, distance);
        }

        Vector3 targetPos = target.position + dir * finalDist;
        transform.position = Vector3.Lerp(transform.position, targetPos, positionSmoothing * Time.deltaTime);

        transform.LookAt(target.position + Vector3.up * 1.0f);
    }
}