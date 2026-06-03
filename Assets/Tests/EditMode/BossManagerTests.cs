using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BossManagerTests
{
    private BossManager manager;
    private GameObject gameObject;

    [SetUp]
    public void SetUp()
    {
        gameObject = new GameObject("BossManager");
        manager = gameObject.AddComponent<BossManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void IsBossDefeated_ReturnsFalse_WhenNoneDefeated()
    {
        Assert.IsFalse(manager.IsBossDefeated("KnightBoss"));
    }

    [Test]
    public void IsBossDefeated_ReturnsFalse_ForNull()
    {
        Assert.IsFalse(manager.IsBossDefeated(null));
    }

    [Test]
    public void IsBossDefeated_ReturnsFalse_ForEmpty()
    {
        Assert.IsFalse(manager.IsBossDefeated(""));
    }

    [Test]
    public void MarkBossAsDefeated_MakesBossDefeated()
    {
        manager.MarkBossAsDefeated("KnightBoss");

        Assert.IsTrue(manager.IsBossDefeated("KnightBoss"));
    }

    [Test]
    public void MarkBossAsDefeated_IgnoresNull()
    {
        manager.MarkBossAsDefeated(null);

        Assert.AreEqual(0, manager.GetDefeatedBosses().Count);
    }

    [Test]
    public void MarkBossAsDefeated_IgnoresEmpty()
    {
        manager.MarkBossAsDefeated("");

        Assert.AreEqual(0, manager.GetDefeatedBosses().Count);
    }

    [Test]
    public void MarkBossAsDefeated_DuplicateIsIdempotent()
    {
        manager.MarkBossAsDefeated("KnightBoss");
        manager.MarkBossAsDefeated("KnightBoss");

        Assert.AreEqual(1, manager.GetDefeatedBosses().Count);
    }

    [Test]
    public void GetDefeatedBosses_ReturnsAll()
    {
        manager.MarkBossAsDefeated("KnightBoss");
        manager.MarkBossAsDefeated("FoxBoss");

        var list = manager.GetDefeatedBosses();
        Assert.AreEqual(2, list.Count);
        Assert.Contains("KnightBoss", list);
        Assert.Contains("FoxBoss", list);
    }

    [Test]
    public void GetDefeatedBosses_ReturnsNewListInstance()
    {
        manager.MarkBossAsDefeated("KnightBoss");

        var list1 = manager.GetDefeatedBosses();
        var list2 = manager.GetDefeatedBosses();
        Assert.AreNotSame(list1, list2);
    }

    [Test]
    public void SetDefeatedBosses_RestoresState()
    {
        manager.SetDefeatedBosses(new List<string> { "KnightBoss", "FoxBoss" });

        Assert.IsTrue(manager.IsBossDefeated("KnightBoss"));
        Assert.IsTrue(manager.IsBossDefeated("FoxBoss"));
    }

    [Test]
    public void SetDefeatedBosses_ClearsPreviousState()
    {
        manager.MarkBossAsDefeated("OldBoss");
        manager.SetDefeatedBosses(new List<string> { "NewBoss" });

        Assert.IsFalse(manager.IsBossDefeated("OldBoss"));
        Assert.IsTrue(manager.IsBossDefeated("NewBoss"));
    }

    [Test]
    public void SetDefeatedBosses_FiltersNullAndEmpty()
    {
        manager.SetDefeatedBosses(new List<string> { "Boss_A", null, "", "Boss_B" });

        Assert.AreEqual(2, manager.GetDefeatedBosses().Count);
    }

    [Test]
    public void SetDefeatedBosses_HandlesNullList()
    {
        manager.MarkBossAsDefeated("Boss_A");
        manager.SetDefeatedBosses(null);

        Assert.AreEqual(0, manager.GetDefeatedBosses().Count);
    }

    [Test]
    public void ClearAllBosses_RemovesAll()
    {
        manager.MarkBossAsDefeated("KnightBoss");
        manager.MarkBossAsDefeated("FoxBoss");

        manager.ClearAllBosses();

        Assert.AreEqual(0, manager.GetDefeatedBosses().Count);
    }
}
