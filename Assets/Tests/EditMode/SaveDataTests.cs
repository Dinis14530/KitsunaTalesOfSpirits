using System.Collections.Generic;
using NUnit.Framework;

public class SaveDataTests
{
    [Test]
    public void NewSaveData_HasDefaultValues()
    {
        var data = new SaveData();

        Assert.AreEqual(0, data.playerHealth);
        Assert.AreEqual(0, data.playerMaxHealth);
        Assert.AreEqual(0, data.coins);
        Assert.IsFalse(data.hasPurchasedMap);
        Assert.IsFalse(data.canDash);
        Assert.IsNull(data.activeCheckpoint);
    }

    [Test]
    public void NewSaveData_ListsAreInitialized()
    {
        var data = new SaveData();

        Assert.IsNotNull(data.inventoryItems);
        Assert.IsNotNull(data.unlockedAbilities);
        Assert.IsNotNull(data.openedChests);
        Assert.IsNotNull(data.openedDoors);
        Assert.IsNotNull(data.defeatedBosses);
    }

    [Test]
    public void NewSaveData_ListsAreEmpty()
    {
        var data = new SaveData();

        Assert.AreEqual(0, data.inventoryItems.Count);
        Assert.AreEqual(0, data.unlockedAbilities.Count);
        Assert.AreEqual(0, data.openedChests.Count);
        Assert.AreEqual(0, data.openedDoors.Count);
        Assert.AreEqual(0, data.defeatedBosses.Count);
    }

    [Test]
    public void SaveData_CanStoreAndRetrieveValues()
    {
        var data = new SaveData
        {
            playerHealth = 5,
            playerMaxHealth = 7,
            coins = 42,
            hasPurchasedMap = true,
            canDash = true,
            activeCheckpoint = "Campfire_02",
        };

        Assert.AreEqual(5, data.playerHealth);
        Assert.AreEqual(7, data.playerMaxHealth);
        Assert.AreEqual(42, data.coins);
        Assert.IsTrue(data.hasPurchasedMap);
        Assert.IsTrue(data.canDash);
        Assert.AreEqual("Campfire_02", data.activeCheckpoint);
    }

    [Test]
    public void SaveData_CanAddItemsToLists()
    {
        var data = new SaveData();

        data.inventoryItems.Add(
            new InventoryItemData
            {
                itemName = "Potion",
                quantity = 3,
                itemDescription = "Heals HP",
            }
        );
        data.unlockedAbilities.Add("Dash");
        data.openedChests.Add("Chest_01");
        data.openedDoors.Add("Door_01");
        data.defeatedBosses.Add("KnightBoss");

        Assert.AreEqual(1, data.inventoryItems.Count);
        Assert.AreEqual("Potion", data.inventoryItems[0].itemName);
        Assert.AreEqual(3, data.inventoryItems[0].quantity);
        Assert.AreEqual(1, data.unlockedAbilities.Count);
        Assert.AreEqual(1, data.openedChests.Count);
        Assert.AreEqual(1, data.openedDoors.Count);
        Assert.AreEqual(1, data.defeatedBosses.Count);
    }
}

public class InventoryItemDataTests
{
    [Test]
    public void NewInventoryItemData_HasDefaultValues()
    {
        var item = new InventoryItemData();

        Assert.IsNull(item.itemName);
        Assert.AreEqual(0, item.quantity);
        Assert.IsNull(item.itemDescription);
    }

    [Test]
    public void InventoryItemData_StoresValues()
    {
        var item = new InventoryItemData
        {
            itemName = "Health Potion",
            quantity = 5,
            itemDescription = "Restores health",
        };

        Assert.AreEqual("Health Potion", item.itemName);
        Assert.AreEqual(5, item.quantity);
        Assert.AreEqual("Restores health", item.itemDescription);
    }
}
