using UnityEngine;

public class MapSellerNPC : MonoBehaviour, IInterectable
{
    [Header("Compra de mapa")]
    [SerializeField] private int mapPrice = 50;
    [SerializeField] private CoinDisplay coinDisplay;
    [SerializeField] private GameObject purchaseUI;
    [SerializeField] private PlayerController player;

    private bool isUIOpen;

    private void Start()
    {
        if (coinDisplay == null)
            coinDisplay = FindFirstObjectByType<CoinDisplay>();

        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (purchaseUI != null)
            purchaseUI.SetActive(false);
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (MapManager.Instance != null && MapManager.Instance.IsMapPurchased())
            return;

        if (isUIOpen)
            ClosePurchaseUI();
        else
            OpenPurchaseUI();
    }

    public void ConfirmPurchase()
    {
        if (MapManager.Instance == null || MapManager.Instance.IsMapPurchased())
        {
            ClosePurchaseUI();
            return;
        }

        if (coinDisplay == null)
            coinDisplay = FindFirstObjectByType<CoinDisplay>();

        if (coinDisplay == null)
            return;

        if (coinDisplay.GetCoins() < mapPrice)
        {
            Debug.LogWarning("Moedas insuficientes para comprar o mapa");
            ClosePurchaseUI();
            return;
        }

        coinDisplay.AddCoins(-mapPrice);
        MapManager.Instance.SetMapPurchased(true);
        ClosePurchaseUI();
    }

    public void CancelPurchase()
    {
        ClosePurchaseUI();
    }

    private void OpenPurchaseUI()
    {
        if (purchaseUI == null)
            return;

        isUIOpen = true;
        purchaseUI.SetActive(true);

        if (player != null)
        {
            player.canMove = false;
            player.isInDialogue = true;
            player.ForceIdle();
        }
    }

    private void ClosePurchaseUI()
    {
        if (purchaseUI != null)
            purchaseUI.SetActive(false);

        isUIOpen = false;

        if (player != null)
        {
            player.canMove = true;
            player.isInDialogue = false;
        }
    }
}
