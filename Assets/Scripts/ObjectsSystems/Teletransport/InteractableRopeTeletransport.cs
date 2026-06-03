using UnityEngine;

public class InteractableTeleport : MonoBehaviour, IInterectable
{
    [Header("Teleport Target")]
    public Transform teleportPoint;
    private bool canInteract = true;

    public void Interact()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null && teleportPoint != null)
        {
            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Teleporta usando Rigidbody2D
                rb.position = teleportPoint.position;
                rb.linearVelocity = Vector2.zero; // zera velocidade para evitar glitches
            }
            else
            {
                // Fallback: se não houver Rigidbody
                playerObj.transform.position = teleportPoint.position;
            }

            GameDebug.Log($"{GlobalHelper.GenerateUniqueID(gameObject)} teleportou o jogador!");
        }
    }

    public bool CanInteract()
    {
        return canInteract;
    }
}
