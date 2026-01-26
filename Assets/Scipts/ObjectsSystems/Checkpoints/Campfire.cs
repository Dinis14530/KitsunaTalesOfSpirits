using UnityEngine;

public class CampfireCheckpoint : MonoBehaviour, IInterectable
{
    private static CampfireCheckpoint activeCheckpoint = null; // checkpoint ativo global
    private Animator animator;
    private bool isActive = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Começa apagado
        SetInactive();
    }

    public bool CanInteract()
    {
        return !isActive;
    }

    public void Interact()
    {
        if (isActive) return;

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.SetCheckpoint(transform.position);
            ActivateCheckpoint();
            Debug.Log("Checkpoint ativado");
        }
    }

    private void ActivateCheckpoint()
    {
        // Desativa o checkpoint anterior
        if (activeCheckpoint != null)
        {
            activeCheckpoint.SetInactive();
        }

        // Ativa este
        isActive = true;
        activeCheckpoint = this;

        if (animator != null)
        {
            animator.SetBool("IsLit", true); // assume que a animação acesa é controlada por bool
        }
    }

    private void SetInactive()
    {
        isActive = false;

        if (animator != null)
        {
            animator.SetBool("IsLit", false);
        }
    }
}
