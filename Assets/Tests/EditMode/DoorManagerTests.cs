using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DoorManagerTests
{
    private DoorManager manager;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("DoorManager");
        manager = gameObject.AddComponent<DoorManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void IsDoorOpened_ReturnsFalse_WhenNoneOpened()
    {
        Assert.IsFalse(manager.IsDoorOpened("Door_01"));
    }

    [Test]
    public void IsDoorOpened_ReturnsFalse_ForNullId()
    {
        Assert.IsFalse(manager.IsDoorOpened(null));
    }

    [Test]
    public void IsDoorOpened_ReturnsFalse_ForEmptyId()
    {
        Assert.IsFalse(manager.IsDoorOpened(""));
    }

    [Test]
    public void MarkDoorAsOpened_MakesDoorOpened()
    {
        manager.MarkDoorAsOpened("Door_01");

        Assert.IsTrue(manager.IsDoorOpened("Door_01"));
    }

    [Test]
    public void MarkDoorAsOpened_IgnoresNull()
    {
        manager.MarkDoorAsOpened(null);

        Assert.AreEqual(0, manager.GetOpenedDoors().Count);
    }

    [Test]
    public void MarkDoorAsOpened_IgnoresEmpty()
    {
        manager.MarkDoorAsOpened("");

        Assert.AreEqual(0, manager.GetOpenedDoors().Count);
    }

    [Test]
    public void MarkDoorAsOpened_DuplicateIsIdempotent()
    {
        manager.MarkDoorAsOpened("Door_01");
        manager.MarkDoorAsOpened("Door_01");

        Assert.AreEqual(1, manager.GetOpenedDoors().Count);
    }

    [Test]
    public void GetOpenedDoors_ReturnsAllOpened()
    {
        manager.MarkDoorAsOpened("Door_01");
        manager.MarkDoorAsOpened("Door_02");

        var list = manager.GetOpenedDoors();
        Assert.AreEqual(2, list.Count);
        Assert.Contains("Door_01", list);
        Assert.Contains("Door_02", list);
    }

    [Test]
    public void SetOpenedDoors_RestoresState()
    {
        manager.SetOpenedDoors(new List<string> { "Door_A", "Door_B" });

        Assert.IsTrue(manager.IsDoorOpened("Door_A"));
        Assert.IsTrue(manager.IsDoorOpened("Door_B"));
    }

    [Test]
    public void SetOpenedDoors_ClearsPreviousState()
    {
        manager.MarkDoorAsOpened("OldDoor");
        manager.SetOpenedDoors(new List<string> { "NewDoor" });

        Assert.IsFalse(manager.IsDoorOpened("OldDoor"));
        Assert.IsTrue(manager.IsDoorOpened("NewDoor"));
    }

    [Test]
    public void SetOpenedDoors_FiltersNullAndEmptyIds()
    {
        manager.SetOpenedDoors(new List<string> { "Door_01", null, "", "Door_02" });

        Assert.AreEqual(2, manager.GetOpenedDoors().Count);
    }

    [Test]
    public void SetOpenedDoors_HandlesNullList()
    {
        manager.MarkDoorAsOpened("Door_01");
        manager.SetOpenedDoors(null);

        Assert.AreEqual(0, manager.GetOpenedDoors().Count);
    }

    [Test]
    public void ClearAllDoors_RemovesAll()
    {
        manager.MarkDoorAsOpened("Door_01");
        manager.MarkDoorAsOpened("Door_02");

        manager.ClearAllDoors();

        Assert.AreEqual(0, manager.GetOpenedDoors().Count);
    }
}
