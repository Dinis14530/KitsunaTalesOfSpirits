using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour, IInterectable
{
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private CoinDisplay coinDisplay;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private CanvasGroup shopUI; 
    public PlayerController player;
    private Dictionary<ItemSO, int> itemsPurchased = new Dictionary<ItemSO, int>();
    private bool isShopOpen = false;

    private void Start()
    {
        // Tenta encontrar o CanvasGroup automaticamente se não estiver atribuído
        if (shopUI == null)
        {
            shopUI = GetComponentInChildren<CanvasGroup>();
        }

        // Garante que a loja comece fechada
        shopUI.alpha = 0;
        shopUI.blocksRaycasts = false;
        shopUI.interactable = false;
        
    }

    // Implementa IInterectable -> chamado quando player pressiona F
    public void Interact()
    {
        if (isShopOpen)
            CloseShop();
        else
            OpenShop();
    }

    public bool CanInteract()
    {
        return true;
    }

    private void OpenShop()
    {  
        isShopOpen = true;
        shopUI.alpha = 1;
        shopUI.blocksRaycasts = true;
        shopUI.interactable = true;
        player.canMove = false;
        player.isInDialogue = true;
        PopulateShopItems();
    }

    private void CloseShop()
    {
        isShopOpen = false;
        shopUI.alpha = 0;
        shopUI.blocksRaycasts = false;
        shopUI.interactable = false;
        player.canMove = true;
        player.isInDialogue = false;
    }
    
    public void PopulateShopItems()
    {

        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopitem = shopItems[i];
            shopSlots[i].Initialize(shopitem.itemSO, shopitem.price);
            shopSlots[i].gameObject.SetActive(true);
        }
        for (int i = shopItems.Count; i < shopSlots.Length; i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    public void TryBuyItem(ItemSO itemSO, int price)
    {
        // Valida se o item existe
        if (itemSO == null)
        {
            return;
        }
        
        // Verifica o limite de compra
        if (itemSO.purchaseLimit > 0)
        {
            if (!itemsPurchased.ContainsKey(itemSO))
                itemsPurchased[itemSO] = 0;
            
            if (itemsPurchased[itemSO] >= itemSO.purchaseLimit)
            {
                Debug.LogWarning($"Limite de compra atingido para {itemSO.itemName} Máximo: {itemSO.purchaseLimit}");
                return;
            }
        }
        
        // Verifica se tem coins suficientes
        if (coinDisplay.GetCoins() < price)
        {
            Debug.LogWarning("Coins insuficientes");
            return;
        }
        
        // Verifica se tem espaço no inventário
        if (!HasSpaceForItem(itemSO))
        {
            Debug.LogWarning("Inventário cheio");
            return;
        }
        
        // Compra o item
        coinDisplay.AddCoins(-price);
        inventoryManager.AddItem(itemSO.itemName, 1, itemSO.sprite, itemSO.itemDescription);
        
        // Registra a compra
        if (!itemsPurchased.ContainsKey(itemSO))
            itemsPurchased[itemSO] = 0;
        itemsPurchased[itemSO]++;
        
        Debug.Log($"{itemSO.itemName} comprado com sucesso! Compras: {itemsPurchased[itemSO]}/{itemSO.purchaseLimit}");
    }
    
    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.itemSlot)
        {
            // Se o slot tem o mesmo item e não está cheio
            if (!slot.isFull && slot.itemName == itemSO.itemName && slot.quantity < ItemSlot.MaxStack)
                return true;
            // Se o slot está vazio
            else if (!slot.isFull)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}