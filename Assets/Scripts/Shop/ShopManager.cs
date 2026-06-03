using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour, IInterectable
{
    [SerializeField]
    private List<ShopItems> shopItems;

    [SerializeField]
    private ShopSlot[] shopSlots;

    [SerializeField]
    private CoinDisplay coinDisplay;

    [SerializeField]
    private InventoryManager inventoryManager;

    [SerializeField]
    private CanvasGroup shopUI;
    public PlayerController player;
    private Dictionary<ItemSO, int> itemsPurchased = new Dictionary<ItemSO, int>();
    private bool isShopOpen = false;

    private void Start()
    {
        if (shopUI == null)
            shopUI = GetComponentInChildren<CanvasGroup>();

        if (shopUI == null)
        {
            Debug.LogError("[ShopManager] shopUI (CanvasGroup) not found. Shop will not work.");
            return;
        }

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
        if (shopUI == null)
        {
            Debug.LogError("[ShopManager] Cannot open shop: shopUI is null.");
            return;
        }

        isShopOpen = true;
        shopUI.alpha = 1;
        shopUI.blocksRaycasts = true;
        shopUI.interactable = true;

        if (player != null)
        {
            player.canMove = false;
            player.isInDialogue = true;
        }

        PopulateShopItems();
    }

    private void CloseShop()
    {
        isShopOpen = false;

        if (shopUI != null)
        {
            shopUI.alpha = 0;
            shopUI.blocksRaycasts = false;
            shopUI.interactable = false;
        }

        if (player != null)
        {
            player.canMove = true;
            player.isInDialogue = false;
        }
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
                Debug.LogWarning(
                    $"Limite de compra atingido para {itemSO.itemName} Máximo: {itemSO.purchaseLimit}"
                );
                return;
            }
        }

        // Verifica se tem coins suficientes
        if (coinDisplay.GetCoins() < price)
        {
            Debug.LogWarning("Coins insuficientes");
            return;
        }

        // Se é um item que se usa imediatamente (maxHealth, health, etc)
        if (IsInstantUseItem(itemSO))
        {
            // Tenta usar o item
            bool used = itemSO.UseItem();
            if (used)
            {
                coinDisplay.AddCoins(-price);

                // Registra a compra
                if (!itemsPurchased.ContainsKey(itemSO))
                    itemsPurchased[itemSO] = 0;
                itemsPurchased[itemSO]++;

                Debug.Log(
                    $"{itemSO.itemName} comprado com sucesso! Compras: {itemsPurchased[itemSO]}/{itemSO.purchaseLimit}"
                );
                return;
            }
            else
            {
                Debug.LogWarning($"{itemSO.itemName} não pode ser usado agora");
                return;
            }
        }

        // Se é um item normal, vai para o inventário
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

        Debug.Log(
            $"{itemSO.itemName} comprado com sucesso! Compras: {itemsPurchased[itemSO]}/{itemSO.purchaseLimit}"
        );
    }

    private bool IsInstantUseItem(ItemSO itemSO)
    {
        // Itens que são usados imediatamente
        return itemSO.statToChange == StatToChange.maxHealth || itemSO.isAbility;
    }

    private bool HasSpaceForItem(ItemSO itemSO)
    {
        foreach (var slot in inventoryManager.itemSlot)
        {
            // Se o slot tem o mesmo item e não está cheio
            if (
                !slot.isFull
                && slot.itemName == itemSO.itemName
                && slot.quantity < ItemSlot.MaxStack
            )
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
