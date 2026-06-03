using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ChestManagerTests
{
    private ChestManager manager;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("ChestManager");
        manager = gameObject.AddComponent<ChestManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void IsChestOpened_ReturnsFalse_WhenNoChestsOpened()
    {
        Assert.IsFalse(manager.IsChestOpened("Chest_01"));
    }

    [Test]
    public void MarkChestAsOpened_MakesChestOpened()
    {
        manager.MarkChestAsOpened("Chest_01");

        Assert.IsTrue(manager.IsChestOpened("Chest_01"));
    }

    [Test]
    public void MarkChestAsOpened_DoesNotAffectOtherChests()
    {
        manager.MarkChestAsOpened("Chest_01");

        Assert.IsFalse(manager.IsChestOpened("Chest_02"));
    }

    [Test]
    public void MarkChestAsOpened_DuplicateIsIdempotent()
    {
        manager.MarkChestAsOpened("Chest_01");
        manager.MarkChestAsOpened("Chest_01");

        var list = manager.GetOpenedChests();
        Assert.AreEqual(1, list.Count);
    }

    [Test]
    public void GetOpenedChests_ReturnsAllOpenedChests()
    {
        manager.MarkChestAsOpened("Chest_01");
        manager.MarkChestAsOpened("Chest_02");
        manager.MarkChestAsOpened("Chest_03");

        var list = manager.GetOpenedChests();
        Assert.AreEqual(3, list.Count);
        Assert.Contains("Chest_01", list);
        Assert.Contains("Chest_02", list);
        Assert.Contains("Chest_03", list);
    }

    [Test]
    public void GetOpenedChests_ReturnsNewListInstance()
    {
        manager.MarkChestAsOpened("Chest_01");

        var list1 = manager.GetOpenedChests();
        var list2 = manager.GetOpenedChests();

        Assert.AreNotSame(list1, list2);
    }

    [Test]
    public void SetOpenedChests_RestoresState()
    {
        var ids = new List<string> { "Chest_A", "Chest_B" };
        manager.SetOpenedChests(ids);

        Assert.IsTrue(manager.IsChestOpened("Chest_A"));
        Assert.IsTrue(manager.IsChestOpened("Chest_B"));
        Assert.IsFalse(manager.IsChestOpened("Chest_C"));
    }

    [Test]
    public void SetOpenedChests_ClearsPreviousState()
    {
        manager.MarkChestAsOpened("OldChest");
        manager.SetOpenedChests(new List<string> { "NewChest" });

        Assert.IsFalse(manager.IsChestOpened("OldChest"));
        Assert.IsTrue(manager.IsChestOpened("NewChest"));
    }

    [Test]
    public void SetOpenedChests_HandlesNullList()
    {
        manager.MarkChestAsOpened("Chest_01");
        manager.SetOpenedChests(null);

        Assert.IsFalse(manager.IsChestOpened("Chest_01"));
        Assert.AreEqual(0, manager.GetOpenedChests().Count);
    }

    [Test]
    public void ClearAllChests_RemovesAll()
    {
        manager.MarkChestAsOpened("Chest_01");
        manager.MarkChestAsOpened("Chest_02");

        manager.ClearAllChests();

        Assert.AreEqual(0, manager.GetOpenedChests().Count);
        Assert.IsFalse(manager.IsChestOpened("Chest_01"));
    }
}
