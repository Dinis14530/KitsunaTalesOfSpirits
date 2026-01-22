using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInterectable interectableInRange = null;
    public GameObject interactionIcon;
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    
    void Update()
    {
        // Verifica se a tecla F foi pressionada
        if (interectableInRange != null && Input.GetKeyDown(KeyCode.F))
        {
            interectableInRange.Interact();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interectableInRange?.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IInterectable interectable) && interectable.CanInteract())
        {
            interectableInRange = interectable;
            interactionIcon.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IInterectable interectable) && interectable == interectableInRange)
        {
            interectableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}
