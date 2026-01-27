using UnityEngine;

public class CampfireCheckpoint : MonoBehaviour, IInterectable
{
    private static CampfireCheckpoint activeCheckpoint = null; 
    private Animator animator;
    private bool isActive = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetInactive(); // começa apagado
    }

    private void Start()
    {
        // Restaura checkpoint do save
        PlayerSave playerSave = FindObjectOfType<PlayerSave>();
        if (playerSave != null && playerSave.currentCheckpoint == gameObject.name)
        {
            ActivateCheckpoint();
        }
    }

    public bool CanInteract()
    {
        return !isActive;
    }

    public void Interact()
    {
        if (isActive) return;

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        PlayerSave playerSave = FindObjectOfType<PlayerSave>();

        if (player != null)
        {
            player.SetCheckpoint(transform.position);
            ActivateCheckpoint();

            // Auto-save
            if (playerSave != null)
            {
                playerSave.currentCheckpoint = gameObject.name;
                playerSave.SaveGame();
            }

            Debug.Log("Checkpoint ativado e jogo guardado");
        }
    }

    private void ActivateCheckpoint()
    {
        if (activeCheckpoint != null)
            activeCheckpoint.SetInactive();

        isActive = true;
        activeCheckpoint = this;

        if (animator != null)
            animator.SetBool("IsLit", true);
    }

    private void SetInactive()
    {
        isActive = false;
        if (animator != null)
            animator.SetBool("IsLit", false);
    }
}
