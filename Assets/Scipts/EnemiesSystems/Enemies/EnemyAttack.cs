using UnityEngine;

// Ataque Inimigo
public class EnemyAttack : MonoBehaviour
{
    public int damage = 1; // Dano
    public float cooldownTime = 2f; // Tempo de cooldown entre ataques

    private float lastAttackTime;

    private void OnCollisionEnter2D(Collision2D collision) // Chama sempre que inimigo colide com o player
    {
        if (collision.gameObject.CompareTag("Player") && Time.time - lastAttackTime > cooldownTime) // Se colidir com a tag "player" e cooldown passou
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>(); // Vida do jopgador
            PlayerKnockBack knockback = collision.gameObject.GetComponent<PlayerKnockBack>(); // Knockback

            if (player != null && !player.isInvincible) // Se o jogador nao estiver invencivel 
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

                lastAttackTime = Time.time; // Atualiza o tempo do último ataque
            }
        }
    }
}
