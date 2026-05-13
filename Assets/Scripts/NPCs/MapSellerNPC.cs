using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSellerNPC : MonoBehaviour, IInterectable
{
    [Header("Compra de mapa")]
    [SerializeField]
    private int mapPrice;

    [SerializeField]
    private CoinDisplay coinDisplay;

    [SerializeField]
    private GameObject purchaseUI;

    [SerializeField]
    private PlayerController player;

    [Header("Diálogo")]
    [SerializeField]
    private TMP_Text dialogueText;

    [TextArea]
    [SerializeField]
    private string purchaseMessage;

    [TextArea]
    [SerializeField]
    private string soldMessage;

    [SerializeField]
    private float soldMessageDuration;

    private bool isUIOpen;
    private Coroutine soldMessageCoroutine;
    private Button[] purchaseButtons;

    private void Start()
    {
        if (coinDisplay == null)
            coinDisplay = FindFirstObjectByType<CoinDisplay>();

        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (purchaseUI != null)
        {
            purchaseUI.SetActive(false);
            if (dialogueText == null)
                dialogueText = purchaseUI.GetComponentInChildren<TMP_Text>(true);

            purchaseButtons = purchaseUI.GetComponentsInChildren<Button>(true);
        }

        if (dialogueText != null)
            dialogueText.text = purchaseMessage;

        SetPurchaseButtonsVisible(!IsMapPurchased());
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (MapManager.Instance != null && MapManager.Instance.IsMapPurchased())
        {
            ShowSoldMessage();
            return;
        }

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

        if (dialogueText != null)
            dialogueText.text = purchaseMessage;

        SetPurchaseButtonsVisible(true);

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
        if (soldMessageCoroutine != null)
        {
            StopCoroutine(soldMessageCoroutine);
            soldMessageCoroutine = null;
        }

        if (purchaseUI != null)
            purchaseUI.SetActive(false);

        isUIOpen = false;

        if (player != null)
        {
            player.canMove = true;
            player.isInDialogue = false;
        }
    }

    private void ShowSoldMessage()
    {
        if (purchaseUI == null)
        {
            Debug.Log(soldMessage);
            return;
        }

        if (dialogueText != null)
            dialogueText.text = soldMessage;

        SetPurchaseButtonsVisible(false);

        if (soldMessageCoroutine != null)
            StopCoroutine(soldMessageCoroutine);

        soldMessageCoroutine = StartCoroutine(ShowSoldMessageRoutine());
    }

    private IEnumerator ShowSoldMessageRoutine()
    {
        purchaseUI.SetActive(true);

        if (player != null)
        {
            player.canMove = false;
            player.isInDialogue = true;
            player.ForceIdle();
        }

        yield return new WaitForSeconds(soldMessageDuration);

        if (purchaseUI != null)
            purchaseUI.SetActive(false);

        if (player != null)
        {
            player.canMove = true;
            player.isInDialogue = false;
        }

        soldMessageCoroutine = null;
    }

    private bool IsMapPurchased()
    {
        return MapManager.Instance != null && MapManager.Instance.IsMapPurchased();
    }

    private void SetPurchaseButtonsVisible(bool visible)
    {
        if (purchaseButtons == null)
            return;

        foreach (Button button in purchaseButtons)
        {
            if (button != null)
                button.gameObject.SetActive(visible);
        }
    }
}
