using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public int damage = 2; // Dano
    public Animator animator; // Animator para a animação
    private bool hasExploded = false; // Flag para evitar múltiplas explosões

    private void OnCollisionEnter2D(Collision2D collision) // Chama sempre que a bomba colide com algo
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Item")) && !hasExploded) // Se colidir com player, enemy ou item e não explodiu ainda
        {
            hasExploded = true; // Marca como explodido

            // Se for player, aplica dano e knockback
            if (collision.gameObject.CompareTag("Player"))
            {
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
            }
            // Se for enemy, aplica dano e knockback
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>(); // Vida do inimigo
                EnemyKnockBack knockback = collision.gameObject.GetComponent<EnemyKnockBack>(); // Knockback

                if (enemy != null)
                {
                    // Aplica dano
                    enemy.TakeDamage(damage);

                    // Aplica knockback no enemy
                    if (knockback != null)
                    {
                        // Calcula direção do knockback 
                        Vector2 direction = (collision.transform.position - transform.position).normalized;

                        knockback.ApplyKnockback(direction); 
                    }
                }
            }

            // Toca a animação de explosão
            animator.SetTrigger("Explode");

            // Destroi o objeto após 1 segundo 
            Destroy(gameObject, 1f);
        }
    }
}

