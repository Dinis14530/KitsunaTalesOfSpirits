using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public int damage = 2; // Dano
    public Animator animator; // Animator para a animação
    private bool hasExploded = false; // Flag para evitar múltiplas explosões

    private void OnCollisionEnter2D(Collision2D collision) // Chama sempre que inimigo colide com o player
    {
        if (collision.gameObject.CompareTag("Player") && !hasExploded) // Se colidir com a tag "player" e não explodiu ainda
        {
            hasExploded = true; // Marca como explodido

            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>(); // Vida do jogador
            PlayerKnockBack knockback = collision.gameObject.GetComponent<PlayerKnockBack>(); // Knockback

            if (player != null && !player.isInvincible) // Se o jogador não estiver invencível 
            {
                // Aplica dano
                player.TakeDamage(damage);

                // Aplica knockback no player
                if (knockback != null)
                {
                    // Calcula direção do knockback 
                    Vector2 direction = (collision.transform.position - transform.position).normalized;

                    knockback.ApplyKnockback(direction); 
                }
            }

            // Toca a animação de explosão
            animator.SetTrigger("Explode");

            // Destroi o objeto após 1 segundo 
            Destroy(gameObject, 1f);
        }
    }
}

