using System;
using System.Collections.Generic;
using UnityEngine;
using Input = UnityEngine.Input;
public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;
    public ItemSO[] itemSOs;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && menuActivated)
        {
            Time.timeScale = 1;
            inventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.E)&& !menuActivated)
        {
            Time.timeScale = 0;
            inventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }

    // Agora retorna o restante que não foi possível agregar ao inventário
    public int AddItem(string itemName, int quantity, Sprite itemSprite, String itemDescription)
    {
        int remaining = quantity;

        if (itemSlot == null || itemSlot.Length == 0)
            return remaining;

        // Primeiro empilha em slots já existentes com o mesmo item
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemName)
            {
                // espera-se que ItemSlot.AddItem retorne o restante 
                remaining = itemSlot[i].AddItem(itemName, remaining, itemSprite, itemDescription);
            }
        }

        // Depois usa slots vazios
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            if (!itemSlot[i].isFull)
            {
                remaining = itemSlot[i].AddItem(itemName, remaining, itemSprite, itemDescription);
            }
        }

        if (remaining > 0)
        {
            Debug.Log("Inventory full");
        }

        return remaining;
    }

    public bool UseItem(string itemName)
    {
        for(int i = 0; i < itemSOs.Length; i++)
        {
            if(itemSOs[i].itemName == itemName)
            {
                Debug.Log("Using item: " + itemName);
                return itemSOs[i].UseItem();
            }
        }
        return false;
    }

    public bool HasItem(ItemSO itemSO)
    {
        if (itemSO == null) return false;
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemSO.itemName && itemSlot[i].quantity > 0)
                return true;
        }
        return false;
    }

    public bool RemoveItem(ItemSO itemSO, int amount)
    {
        if (itemSO == null || amount <= 0) return false;

        int remaining = amount;
        for (int i = 0; i < itemSlot.Length && remaining > 0; i++)
        {
            if (itemSlot[i].isFull && itemSlot[i].itemName == itemSO.itemName)
            {
                remaining = itemSlot[i].RemoveQuantity(remaining);
            }
        }

        if (remaining <= 0)
            return true;

        return false;
    }

    public List<InventoryItemData> ExportInventory()
    {
        List<InventoryItemData> list = new List<InventoryItemData>();
        foreach (var slot in itemSlot)
        {
            if (slot.isFull && slot.quantity > 0)
            {
                list.Add(new InventoryItemData 
                { 
                    itemName = slot.itemName, 
                    quantity = slot.quantity,
                    itemDescription = slot.itemDescription
                });
            }
        }
        return list;
    }

    public void ImportInventory(List<InventoryItemData> items)
    {
        // Limpa o inventário atual
        foreach (var slot in itemSlot)
        {
            slot.quantity = 0;
            slot.isFull = false;
            slot.itemName = "";
            slot.itemSprite = slot.emptySprite;
            slot.itemDescription = "";
            slot.quantityText.enabled = false;
            slot.itemImage.sprite = slot.emptySprite;
        }

        // Adiciona os itens salvos
        foreach (var item in items)
        {
            // Encontra o ItemSO para obter a sprite
            Sprite itemSprite = null;
            foreach (var itemSO in itemSOs)
            {
                if (itemSO.itemName == item.itemName)
                {
                    itemSprite = itemSO.sprite;
                    break;
                }
            }
            
            AddItem(item.itemName, item.quantity, itemSprite, item.itemDescription);
        }
    }

    public void DeselectAllSlots()
    {
        for(int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);    
            itemSlot[i].thisItemSelected = false;   
        }
    }

    
}