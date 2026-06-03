using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManagerTests
{
    private Canvas canvas;
    private InventoryManager inventory;
    private GameObject inventoryGO;

    [SetUp]
    public void SetUp()
    {
        var canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();

        inventoryGO = new GameObject("InventoryCanvas");
        inventoryGO.transform.SetParent(canvasGO.transform);
        inventory = inventoryGO.AddComponent<InventoryManager>();
        inventory.inventoryMenu = new GameObject("Menu");
        inventory.inventoryMenu.transform.SetParent(canvasGO.transform);

        inventory.itemSlot = new ItemSlot[3];
        for (int i = 0; i < 3; i++)
        {
            inventory.itemSlot[i] = CreateSlot(canvasGO.transform);
        }

        inventory.itemSOs = new ItemSO[0];
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(canvas.gameObject);
    }

    private ItemSlot CreateSlot(Transform parent)
    {
        var slotGO = new GameObject("Slot");
        slotGO.transform.SetParent(parent);
        var slot = slotGO.AddComponent<ItemSlot>();

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(parent);
        slot.quantityText = textGO.AddComponent<TextMeshProUGUI>();

        var imageGO = new GameObject("Image");
        imageGO.transform.SetParent(parent);
        slot.itemImage = imageGO.AddComponent<Image>();

        slot.emptySprite = null;
        return slot;
    }

    [Test]
    public void AddItem_PlacesItemInFirstEmptySlot()
    {
        int remaining = inventory.AddItem("Potion", 3, null, "Heals HP");

        Assert.AreEqual(0, remaining);
        Assert.AreEqual("Potion", inventory.itemSlot[0].itemName);
        Assert.AreEqual(3, inventory.itemSlot[0].quantity);
    }

    [Test]
    public void AddItem_StacksOnExistingSlot()
    {
        inventory.AddItem("Potion", 3, null, "Heals HP");
        int remaining = inventory.AddItem("Potion", 2, null, "Heals HP");

        Assert.AreEqual(0, remaining);
        Assert.AreEqual(5, inventory.itemSlot[0].quantity);
    }

    [Test]
    public void AddItem_OverflowsToNextSlot()
    {
        inventory.AddItem("Potion", 9, null, "Heals HP");
        int remaining = inventory.AddItem("Potion", 5, null, "Heals HP");

        Assert.AreEqual(0, remaining);
        Assert.AreEqual(9, inventory.itemSlot[0].quantity);
        Assert.AreEqual(5, inventory.itemSlot[1].quantity);
    }

    [Test]
    public void AddItem_ReturnsRemainder_WhenFull()
    {
        inventory.AddItem("Potion", 9, null, "Heals HP");
        inventory.AddItem("Potion", 9, null, "Heals HP");
        inventory.AddItem("Potion", 9, null, "Heals HP");

        int remaining = inventory.AddItem("Potion", 5, null, "Heals HP");

        Assert.AreEqual(5, remaining);
    }

    [Test]
    public void AddItem_DifferentItems_UseSeparateSlots()
    {
        inventory.AddItem("Potion", 2, null, "Heals");
        inventory.AddItem("Sword", 1, null, "Sharp");

        Assert.AreEqual("Potion", inventory.itemSlot[0].itemName);
        Assert.AreEqual("Sword", inventory.itemSlot[1].itemName);
    }

    [Test]
    public void AddItem_NullSlotArray_ReturnsQuantity()
    {
        inventory.itemSlot = null;

        int remaining = inventory.AddItem("Potion", 3, null, "Heals HP");

        Assert.AreEqual(3, remaining);
    }

    [Test]
    public void AddItem_EmptySlotArray_ReturnsQuantity()
    {
        inventory.itemSlot = new ItemSlot[0];

        int remaining = inventory.AddItem("Potion", 3, null, "Heals HP");

        Assert.AreEqual(3, remaining);
    }

    [Test]
    public void ExportInventory_ReturnsFilledSlots()
    {
        inventory.AddItem("Potion", 3, null, "Heals HP");
        inventory.AddItem("Key", 1, null, "Opens door");

        List<InventoryItemData> exported = inventory.ExportInventory();

        Assert.AreEqual(2, exported.Count);
        Assert.AreEqual("Potion", exported[0].itemName);
        Assert.AreEqual(3, exported[0].quantity);
        Assert.AreEqual("Key", exported[1].itemName);
        Assert.AreEqual(1, exported[1].quantity);
    }

    [Test]
    public void ExportInventory_SkipsEmptySlots()
    {
        inventory.AddItem("Potion", 3, null, "Heals HP");

        List<InventoryItemData> exported = inventory.ExportInventory();

        Assert.AreEqual(1, exported.Count);
    }

    [Test]
    public void HasItem_ReturnsTrue_WhenItemExists()
    {
        var itemSO = ScriptableObject.CreateInstance<ItemSO>();
        itemSO.itemName = "Potion";

        inventory.AddItem("Potion", 3, null, "Heals HP");

        Assert.IsTrue(inventory.HasItem(itemSO));

        Object.DestroyImmediate(itemSO);
    }

    [Test]
    public void HasItem_ReturnsFalse_WhenItemNotPresent()
    {
        var itemSO = ScriptableObject.CreateInstance<ItemSO>();
        itemSO.itemName = "Sword";

        inventory.AddItem("Potion", 3, null, "Heals HP");

        Assert.IsFalse(inventory.HasItem(itemSO));

        Object.DestroyImmediate(itemSO);
    }

    [Test]
    public void HasItem_ReturnsFalse_ForNull()
    {
        Assert.IsFalse(inventory.HasItem(null));
    }

    [Test]
    public void RemoveItem_RemovesFromSlot()
    {
        var itemSO = ScriptableObject.CreateInstance<ItemSO>();
        itemSO.itemName = "Potion";

        inventory.AddItem("Potion", 5, null, "Heals HP");

        bool removed = inventory.RemoveItem(itemSO, 2);

        Assert.IsTrue(removed);
        Assert.AreEqual(3, inventory.itemSlot[0].quantity);

        Object.DestroyImmediate(itemSO);
    }

    [Test]
    public void RemoveItem_ReturnsFalse_WhenNotEnough()
    {
        var itemSO = ScriptableObject.CreateInstance<ItemSO>();
        itemSO.itemName = "Potion";

        inventory.AddItem("Potion", 2, null, "Heals HP");

        bool removed = inventory.RemoveItem(itemSO, 5);

        Assert.IsFalse(removed);

        Object.DestroyImmediate(itemSO);
    }

    [Test]
    public void RemoveItem_ReturnsFalse_ForNull()
    {
        Assert.IsFalse(inventory.RemoveItem(null, 1));
    }

    [Test]
    public void RemoveItem_ReturnsFalse_ForZeroAmount()
    {
        var itemSO = ScriptableObject.CreateInstance<ItemSO>();
        itemSO.itemName = "Potion";

        Assert.IsFalse(inventory.RemoveItem(itemSO, 0));

        Object.DestroyImmediate(itemSO);
    }
}
