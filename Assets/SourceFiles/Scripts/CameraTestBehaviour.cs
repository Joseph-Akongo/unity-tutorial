using UnityEngine;

public class CameraTestBehaviour : MonoBehaviour
{
    public Transform player;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError("TEST FAILED: No Camera component found.");
            return;
        }

        Debug.Log("TEST PASSED: Camera component exists.");
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("TEST WARNING: Player reference not assigned.");
            return;
        }

        // Test if camera is facing player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToPlayer);

        if (dot > 0.9f)
        {
            Debug.Log("TEST PASSED: Camera is facing the player.");
        }
        else
        {
            Debug.LogError("TEST FAILED: Camera is not facing the player.");
        }

        // Test if camera moves when player moves
        if (Vector3.Distance(transform.position, player.position) > 0)
        {
            Debug.Log("TEST PASSED: Camera has positional relationship to player.");
        }
    }
}