using UnityEngine;

public class Door : MonoBehaviour, IInterectable
{
    [Header("Config")]
    public ItemSO requiredItem;

    private bool isOpen = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private InventoryManager inventory;

    [Header("Áudio")]
    public AudioSource audioSource;   // Fonte de som da porta
    public AudioClip openClip;        // Som ao abrir a porta

    [System.Obsolete]
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
        inventory = FindObjectOfType<InventoryManager>();

        // Se não tiver AudioSource, tenta pegar
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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

        Debug.Log("Door opened");
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && openClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // pitch aleatório
            audioSource.PlayOneShot(openClip);
        }
    }
}
