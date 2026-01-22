using UnityEngine;

public class SpikesDamage : MonoBehaviour
{
    public Vector2 teleportPosition;   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Dano dependendo do tipo de objeto (player ou inimigo).
        if (collision.CompareTag("Player"))
        {
            var playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            // teletransportar apenas o player
            collision.transform.position = teleportPosition;
        }
        else if (collision.CompareTag("Enemy"))
        {
            // Tentar componente específico de inimigo
            var enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Mata inimigo instantaneamente
                enemy.TakeDamage(enemy.health);
                return;
            }

            var genericHealth = collision.GetComponent<PlayerHealth>();
            if (genericHealth != null)
            {
                genericHealth.TakeDamage(1);
            }
        }
        else if (collision.CompareTag("Item"))
        {
            Destroy(collision.gameObject);
        }
    }
}
