using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotTests
{
    private Canvas canvas;
    private ItemSlot slot;
    private GameObject slotGO;

    [SetUp]
    public void SetUp()
    {
        var canvasGO = new GameObject("Canvas");
        canvas = canvasGO.AddComponent<Canvas>();

        slotGO = new GameObject("Slot");
        slotGO.transform.SetParent(canvasGO.transform);
        slot = slotGO.AddComponent<ItemSlot>();

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(canvasGO.transform);
        slot.quantityText = textGO.AddComponent<TextMeshProUGUI>();

        var imageGO = new GameObject("Image");
        imageGO.transform.SetParent(canvasGO.transform);
        slot.itemImage = imageGO.AddComponent<Image>();

        slot.emptySprite = null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(canvas.gameObject);
    }

    [Test]
    public void AddItem_ToEmptySlot_SetsNameAndQuantity()
    {
        int remaining = slot.AddItem("Potion", 3, null, "Heals HP");

        Assert.AreEqual(0, remaining);
        Assert.AreEqual("Potion", slot.itemName);
        Assert.AreEqual(3, slot.quantity);
        Assert.IsTrue(slot.isFull);
    }

    [Test]
    public void AddItem_ReturnsZero_WhenQuantityIsZero()
    {
        int remaining = slot.AddItem("Potion", 0, null, "Heals HP");

        Assert.AreEqual(0, remaining);
        Assert.IsFalse(slot.isFull);
    }

    [Test]
    public void AddItem_CapsAtMaxStack()
    {
        int remaining = slot.AddItem("Potion", 15, null, "Heals HP");

        Assert.AreEqual(15 - ItemSlot.MaxStack, remaining);
        Assert.AreEqual(ItemSlot.MaxStack, slot.quantity);
    }

    [Test]
    public void AddItem_StacksOnExistingItem()
    {
        slot.AddItem("Potion", 3, null, "Heals HP");

        int remaining = slot.AddItem("Potion", 4, null, "Heals HP");

        Assert.AreEqual(0, remaining);
        Assert.AreEqual(7, slot.quantity);
    }

    [Test]
    public void AddItem_StackOverflow_ReturnsRemainder()
    {
        slot.AddItem("Potion", 7, null, "Heals HP");

        int remaining = slot.AddItem("Potion", 5, null, "Heals HP");

        Assert.AreEqual(3, remaining);
        Assert.AreEqual(ItemSlot.MaxStack, slot.quantity);
    }

    [Test]
    public void AddItem_DifferentItem_ReturnsAllAsRemainder()
    {
        slot.AddItem("Potion", 3, null, "Heals HP");

        int remaining = slot.AddItem("Sword", 2, null, "A weapon");

        Assert.AreEqual(2, remaining);
        Assert.AreEqual("Potion", slot.itemName);
    }

    [Test]
    public void RemoveQuantity_RemovesSome()
    {
        slot.AddItem("Potion", 5, null, "Heals HP");

        int remaining = slot.RemoveQuantity(2);

        Assert.AreEqual(0, remaining);
        Assert.AreEqual(3, slot.quantity);
        Assert.IsTrue(slot.isFull);
    }

    [Test]
    public void RemoveQuantity_RemovesAll_EmptiesSlot()
    {
        slot.AddItem("Potion", 3, null, "Heals HP");

        int remaining = slot.RemoveQuantity(3);

        Assert.AreEqual(0, remaining);
        Assert.AreEqual(0, slot.quantity);
        Assert.IsFalse(slot.isFull);
        Assert.AreEqual("", slot.itemName);
    }

    [Test]
    public void RemoveQuantity_MoreThanAvailable_EmptiesSlotAndReturnsZero()
    {
        slot.AddItem("Potion", 2, null, "Heals HP");

        int remaining = slot.RemoveQuantity(5);

        Assert.AreEqual(3, remaining);
        Assert.AreEqual(0, slot.quantity);
        Assert.IsFalse(slot.isFull);
    }

    [Test]
    public void RemoveQuantity_OnEmptySlot_ReturnsFullAmount()
    {
        int remaining = slot.RemoveQuantity(5);

        Assert.AreEqual(5, remaining);
    }

    [Test]
    public void RemoveQuantity_ZeroAmount_ReturnsZero()
    {
        slot.AddItem("Potion", 3, null, "Heals HP");

        int remaining = slot.RemoveQuantity(0);

        Assert.AreEqual(0, remaining);
        Assert.AreEqual(3, slot.quantity);
    }

    [Test]
    public void MaxStack_IsNine()
    {
        Assert.AreEqual(9, ItemSlot.MaxStack);
    }
}
