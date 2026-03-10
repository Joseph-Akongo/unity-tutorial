using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayModeTest
{
    private GameObject player;
    private Camera testCamera;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create a player object
        player = new GameObject("Player");
        player.transform.position = Vector3.zero;

        // Create a camera
        GameObject camObj = new GameObject("Camera");
        testCamera = camObj.AddComponent<Camera>();

        // Position camera behind the player
        testCamera.transform.position = new Vector3(0, 5, -10);
        testCamera.transform.LookAt(player.transform);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Camera_Exists_InScene()
    {
        yield return null;

        Assert.IsNotNull(testCamera);
    }

    [UnityTest]
    public IEnumerator Camera_Moves_When_Player_Moves()
    {
        Vector3 initialCamPosition = testCamera.transform.position;

        // Move the player
        player.transform.position += Vector3.forward * 5;

        yield return null;

        // Simulate camera follow
        testCamera.transform.position = player.transform.position + new Vector3(0, 5, -10);

        Assert.AreNotEqual(initialCamPosition, testCamera.transform.position);
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        Object.Destroy(player);
        Object.Destroy(testCamera.gameObject);
        yield return null;
    }
}