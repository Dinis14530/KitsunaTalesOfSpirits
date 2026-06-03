using NUnit.Framework;
using UnityEngine;

public class EnemyTests
{
    private Enemy enemy;
    private GameObject enemyGO;

    [SetUp]
    public void SetUp()
    {
        enemyGO = new GameObject("Enemy");
        enemy = enemyGO.AddComponent<Enemy>();
        enemy.health = 5;
    }

    [TearDown]
    public void TearDown()
    {
        if (enemyGO != null)
            Object.DestroyImmediate(enemyGO);
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        enemy.TakeDamage(2);

        Assert.AreEqual(3, enemy.health);
    }

    [Test]
    public void TakeDamage_MultipleTimes_AccumulatesCorrectly()
    {
        enemy.TakeDamage(1);
        enemy.TakeDamage(2);

        Assert.AreEqual(2, enemy.health);
    }

    [Test]
    public void TakeDamage_FatalDamage_DestroysGameObject()
    {
        enemy.lootDrops = null;

        enemy.TakeDamage(5);

        Assert.IsTrue(enemyGO == null);
    }

    [Test]
    public void TakeDamage_OverKill_DestroysGameObject()
    {
        enemy.lootDrops = null;

        enemy.TakeDamage(100);

        Assert.IsTrue(enemyGO == null);
    }

    [Test]
    public void TakeDamage_NonFatal_KeepsGameObjectAlive()
    {
        enemy.TakeDamage(3);

        Assert.IsFalse(enemyGO == null);
        Assert.AreEqual(2, enemy.health);
    }

    [Test]
    public void TakeDamage_ZeroDamage_NoEffect()
    {
        enemy.TakeDamage(0);

        Assert.AreEqual(5, enemy.health);
        Assert.IsFalse(enemyGO == null);
    }
}
