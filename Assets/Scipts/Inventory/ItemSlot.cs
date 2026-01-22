using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public const int MaxStack = 9;

    // Item Data
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    
    // Item Slot
    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

    // Item Description Slot
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public int AddItem(string itemName, int addQuantity, Sprite itemSprite, string itemDescription)
    {
        if (addQuantity <= 0) return 0;

        // Se o slot já contém o mesmo item, empilha até MaxStack
        if (isFull && this.itemName == itemName)
        {
            int space = MaxStack - this.quantity;
            int toAdd = Math.Min(space, addQuantity);
            this.quantity += toAdd;
            addQuantity -= toAdd;

            quantityText.gameObject.SetActive(true);
            quantityText.text = this.quantity.ToString();
            itemImage.gameObject.SetActive(true);
            return addQuantity; 
        }

        if (!isFull)
        {
            int toAdd = Math.Min(MaxStack, addQuantity);
            quantityText.gameObject.SetActive(true);
            itemImage.gameObject.SetActive(true);

            this.itemName = itemName;
            this.quantity = toAdd;
            this.itemSprite = itemSprite;
            this.itemDescription = itemDescription;
            isFull = true;

            quantityText.text = this.quantity.ToString();
            quantityText.enabled = true;
            itemImage.sprite = itemSprite;

            addQuantity -= toAdd;
            return addQuantity; 
        }

        return addQuantity;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        if (thisItemSelected)
        {
            bool used = inventoryManager.UseItem(itemName);
            if (used)
            {
                quantity -= 1;
                quantityText.text = quantity.ToString();
                if(quantity <= 0)
                    EmptySlot();
            }
            else
            {
                Debug.Log("Item cannot be used right now");
            }
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            itemDescriptionImage.sprite = itemSprite;
            if(itemDescriptionImage.sprite == null)
            {
                itemDescriptionImage.sprite = emptySprite;
            }
        }
    }

    private void EmptySlot()
    {
        quantityText.enabled = false;
        itemImage.sprite = emptySprite;

        itemName = "";
        itemSprite = emptySprite;
        itemDescription = "";
        quantity = 0;
        isFull = false;

        ItemDescriptionNameText.text = "";
        ItemDescriptionText.text = "";
        itemDescriptionImage.sprite = emptySprite;
        
    }

    // Remove a given amount from this slot
    public int RemoveQuantity(int amount)
    {
        if (!isFull || amount <= 0) return amount;

        int toRemove = Math.Min(amount, this.quantity);
        this.quantity -= toRemove;

        if (this.quantity <= 0)
        {
            EmptySlot();
        }
        else
        {
            quantityText.text = this.quantity.ToString();
        }

        return amount - toRemove;
    }

    public void OnRightClick()
    {

    }
}