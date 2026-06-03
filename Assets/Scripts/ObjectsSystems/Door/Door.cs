using TMPro;
using UnityEngine;

public class Door : MonoBehaviour, IInterectable
{
    [Header("Config")]
    public ItemSO requiredItem;

    private bool isOpen = false;
    public string DoorID { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private InventoryManager inventory;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openClip;

    [Header("UI")]
    public GameObject missingKeyPanel;
    public TMP_Text missingKeyText;
    public float panelDuration = 2f;

    private Coroutine missingKeyCoroutine;

    void Awake()
    {
        // Gera ID unico baseado no nome e posicao
        DoorID = gameObject.name + "_" + transform.position.ToString();
        Debug.Log($"Porta criada com ID: {DoorID}");
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
        inventory = FindFirstObjectByType<InventoryManager>();

        if (missingKeyPanel != null)
            missingKeyPanel.SetActive(false);

        // Se nao tiver AudioSource, tenta pegar
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Verifica se esta porta ja foi aberta antes
        if (DoorManager.Instance != null)
        {
            if (DoorManager.Instance.IsTracked(DoorID))
            {
                Debug.Log($"Porta {DoorID} ja foi aberta, mantendo aberta");
                SetOpenedWithoutRemoving();
            }
        }
    }

    public bool CanInteract()
    {
        return !isOpen;
    }

    public void Interact()
    {
        if (isOpen)
            return;

        if (inventory != null && inventory.HasItem(requiredItem))
        {
            // Remove item e abre a porta
            bool removed = inventory.RemoveItem(requiredItem, 1);
            if (removed)
                OpenDoor();
        }
        else
        {
            Debug.Log("You need: " + requiredItem.itemName);
            ShowMissingKeyPanel();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;

        spriteRenderer.enabled = false;
        doorCollider.enabled = false;

        // Toca som de abertura
        AudioHelper.PlayWithRandomPitch(audioSource, openClip);

        // Registra que esta porta foi aberta
        if (DoorManager.Instance != null)
            DoorManager.Instance.MarkTracked(DoorID);

        Debug.Log("Door opened");
    }

    // Versao que nao remove item
    private void SetOpenedWithoutRemoving()
    {
        isOpen = true;
        spriteRenderer.enabled = false;
        doorCollider.enabled = false;
    }

    private void ShowMissingKeyPanel()
    {
        string requiredItemName = requiredItem != null ? requiredItem.itemName : "chave";
        missingKeyCoroutine = TimedPanelHelper.Show(
            this,
            missingKeyPanel,
            panelDuration,
            missingKeyCoroutine,
            missingKeyText,
            "Precisas da chave: " + requiredItemName
        );
    }

    private void OnDisable()
    {
        TimedPanelHelper.Cleanup(this, missingKeyPanel, ref missingKeyCoroutine);
    }
}
