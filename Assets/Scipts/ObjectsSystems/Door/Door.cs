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

    [Header("Áudio")]
    public AudioSource audioSource;
    public AudioClip openClip;

    void Awake()
    {
        // Gera ID único baseado no nome e posição
        DoorID = gameObject.name + "_" + transform.position.ToString();
        Debug.Log($"Porta criada com ID: {DoorID}");
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
        inventory = FindObjectOfType<InventoryManager>();

        // Se não tiver AudioSource, tenta pegar
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Verifica se esta porta já foi aberta antes
        if (DoorManager.Instance != null)
        {
            if (DoorManager.Instance.IsDoorOpened(DoorID))
            {
                Debug.Log($"Porta {DoorID} já foi aberta, mantendo aberta");
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
        if (isOpen) return;

        if (inventory.HasItem(requiredItem))
        {
            // Remove item e abre a porta
            bool removed = inventory.RemoveItem(requiredItem, 1);
            if (removed)
                OpenDoor();
        }
        else
        {
            Debug.Log("You need: " + requiredItem.itemName);
        }
    }

    private void OpenDoor()
    {
        isOpen = true;

        spriteRenderer.enabled = false;
        doorCollider.enabled = false;

        // Toca som de abertura
        PlayOpenSound();

        // Registra que esta porta foi aberta
        if (DoorManager.Instance != null)
            DoorManager.Instance.MarkDoorAsOpened(DoorID);

        Debug.Log("Door opened");
    }

    // Versão que não remove item (usada ao carregar jogo)
    private void SetOpenedWithoutRemoving()
    {
        isOpen = true;
        spriteRenderer.enabled = false;
        doorCollider.enabled = false;
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && openClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(openClip);
        }
    }
}
