using NUnit.Framework;
using UnityEngine;

public class PooledObjectTests
{
    private PooledObject pooledObject;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("PooledObject");
        pooledObject = gameObject.AddComponent<PooledObject>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void SetPoolId_StoresId()
    {
        pooledObject.SetPoolId(42);

        Assert.AreEqual(42, pooledObject.PoolId);
    }

    [Test]
    public void TryMarkAsReleased_ReturnsFalse_WhenAlreadyReleased()
    {
        Assert.IsFalse(pooledObject.TryMarkAsReleased());
    }

    [Test]
    public void MarkAsInUse_ThenRelease_ReturnsTrue()
    {
        pooledObject.MarkAsInUse();

        Assert.IsTrue(pooledObject.TryMarkAsReleased());
    }

    [Test]
    public void DoubleRelease_ReturnsFalseOnSecond()
    {
        pooledObject.MarkAsInUse();

        Assert.IsTrue(pooledObject.TryMarkAsReleased());
        Assert.IsFalse(pooledObject.TryMarkAsReleased());
    }

    [Test]
    public void MarkAsInUse_CanBeCalledMultipleTimes()
    {
        pooledObject.MarkAsInUse();
        pooledObject.MarkAsInUse();

        Assert.IsTrue(pooledObject.TryMarkAsReleased());
    }

    [Test]
    public void ReuseAfterRelease_WorksCorrectly()
    {
        pooledObject.MarkAsInUse();
        pooledObject.TryMarkAsReleased();

        pooledObject.MarkAsInUse();
        Assert.IsTrue(pooledObject.TryMarkAsReleased());
    }
}
