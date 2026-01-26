using UnityEngine;

public class CampfireCheckpoint : MonoBehaviour, IInterectable
{
    private bool activated = false;

    public bool CanInteract()
    {
        return !activated;
    }

    public void Interact()
    {
        if (activated) return;

        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.SetCheckpoint(transform.position);
            activated = true;

            Debug.Log("Checkpoint ativado");
        }
    }
}
