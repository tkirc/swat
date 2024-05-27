using System.Drawing.Printing;
using NUnit.Framework;
using UnityEngine;
using Unity.Netcode;
using log4net.Util;

public class LandmineTests
{
    private GameObject landmineObject;
    private Landmine landmine;

    [SetUp]
    public void Setup()
    {
        landmineObject = new GameObject();
        landmine = landmineObject.AddComponent<Landmine>();

        landmineObject.AddComponent<NetworkObject>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(landmineObject);
    }

    [Test]
    public void TestShoot()
    {
        landmine.shoot();
        Assert.IsNull(landmineObject.transform.parent, "Landmine should not have a parent after shooting.");

        Assert.IsNotNull(landmineObject.GetComponent<Rigidbody>(), "Landmine should have a Rigidbody component after shooting.");
    }
}
