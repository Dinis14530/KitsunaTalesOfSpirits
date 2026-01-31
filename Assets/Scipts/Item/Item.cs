using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemSO itemSO; 
    private int quantity = 1; 
    private InventoryManager inventoryManager;
    private CoinDisplay coinDisplay;
    private AbilityUIManager abilityUIManager;
    private float nextPickupTime;

    [Header("Áudio")]
    public AudioSource audioSource;   // Fonte de som
    public AudioClip pickupClip;      // Som ao pegar o item

    void Start()
    {
        coinDisplay = FindFirstObjectByType<CoinDisplay>();
        inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();
        abilityUIManager = FindFirstObjectByType<AbilityUIManager>();

        // Se não tiver AudioSource, tenta pegar um
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void SetItemData(ItemSO itemSO_, int qty)
    {
        itemSO = itemSO_;
        quantity = qty;
    }

    private void TryPickup()
    {
        if (itemSO == null) return;

        // Toca som de pickup
        PlayPickupSound();

        // Moeda
        if (itemSO.isCurrency)
        {
            coinDisplay.AddCoins(itemSO.coinValue * quantity);
            Destroy(gameObject);
            return;
        }

        // Habilidade
        if (itemSO.isAbility)
        {
            itemSO.UseItem();

            if (abilityUIManager != null)
            {
                abilityUIManager.ShowAbilityUI(itemSO.sprite, itemSO.itemName, 3f); // 3 segundos
            }

            Destroy(gameObject);
            return;
        }

        // Item normal
        if (inventoryManager != null)
        {
            int remaining = inventoryManager.AddItem(itemSO.itemName, quantity, itemSO.sprite, itemSO.itemDescription);
            
            if (remaining <= 0)
                Destroy(gameObject);
            else
                quantity = remaining;
        }
    }

    private void PlayPickupSound()
    {
        if (audioSource != null && pickupClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // pitch aleatório
            audioSource.PlayOneShot(pickupClip);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        TryPickup();
        nextPickupTime = Time.time;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time < nextPickupTime) return;

        TryPickup();
        nextPickupTime = Time.time;
    }
}
