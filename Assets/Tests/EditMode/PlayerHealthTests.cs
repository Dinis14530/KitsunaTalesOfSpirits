using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthTests
{
    private PlayerHealth playerHealth;
    private GameObject playerGO;
    private Canvas canvas;

    [SetUp]
    public void SetUp()
    {
        var canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();

        playerGO = new GameObject("Player");
        playerHealth = playerGO.AddComponent<PlayerHealth>();

        var healthDisplayGO = new GameObject("HealthDisplay");
        healthDisplayGO.transform.SetParent(canvasGO.transform);
        var healthDisplay = healthDisplayGO.AddComponent<HealthDisplay>();

        var imageGO = new GameObject("HealthImage");
        imageGO.transform.SetParent(canvasGO.transform);
        healthDisplay.healthImg = imageGO.AddComponent<Image>();

        playerHealth.healthDisplay = healthDisplay;
        playerHealth.maxHealth = 5;
        playerHealth.currentHealth = 5;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerGO);
        Object.DestroyImmediate(canvas.gameObject);
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        playerHealth.TakeDamage(2);

        Assert.AreEqual(3, playerHealth.currentHealth);
    }

    [Test]
    public void TakeDamage_ClampsAtZero()
    {
        playerHealth.TakeDamage(10);

        Assert.AreEqual(0, playerHealth.currentHealth);
    }

    [Test]
    public void TakeDamage_Blocked_WhenInvincible()
    {
        playerHealth.isInvincible = true;

        playerHealth.TakeDamage(3);

        Assert.AreEqual(5, playerHealth.currentHealth);
    }

    [Test]
    public void TakeDamage_FatalDamage_ResetsToCheckpoint()
    {
        playerHealth.SetCheckpoint(new Vector3(10, 20, 0));

        playerHealth.TakeDamage(5);

        Assert.AreEqual(playerHealth.maxHealth, playerHealth.currentHealth);
        Assert.AreEqual(new Vector3(10, 20, 0), playerGO.transform.position);
    }

    [Test]
    public void SetCheckpoint_StoresPosition()
    {
        var checkpoint = new Vector3(5, 10, 0);
        playerHealth.SetCheckpoint(checkpoint);

        playerHealth.TakeDamage(100);

        Assert.AreEqual(checkpoint, playerGO.transform.position);
    }

    [Test]
    public void RestoreHealth_SetsCurrentHealth()
    {
        playerHealth.TakeDamage(3);

        playerHealth.RestoreHealth(5);

        Assert.AreEqual(5, playerHealth.currentHealth);
    }

    [Test]
    public void TakeDamage_ZeroAmount_KeepsHealth()
    {
        playerHealth.TakeDamage(0);

        Assert.AreEqual(5, playerHealth.currentHealth);
    }

    [Test]
    public void MultipleDamageHits_AccumulateCorrectly()
    {
        playerHealth.TakeDamage(1);
        playerHealth.TakeDamage(2);

        Assert.AreEqual(2, playerHealth.currentHealth);
    }
}
