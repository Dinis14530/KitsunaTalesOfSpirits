using UnityEngine;

public class Door : MonoBehaviour, IInterectable
{
    [Header("Config")]
    public ItemSO requiredItem;

    private bool isOpen = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;
    private InventoryManager inventory;

    [System.Obsolete]
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();
        inventory = FindObjectOfType<InventoryManager>();
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

        Debug.Log("Door opened");
    }
}
