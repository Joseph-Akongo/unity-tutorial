using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NewTestScript
{
    private GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject();
        playerObject.AddComponent<Rigidbody>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
    }

    [Test]
    public void GameObject_HasRigidbody()
    {
        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        Assert.IsNotNull(rb);
    }

    [UnityTest]
    public IEnumerator GameObject_ExistsNextFrame()
    {
        yield return null;
        Assert.IsNotNull(playerObject);
    }
}