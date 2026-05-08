using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    public int damage = 2; // Dano
    public Animator animator; // Animator para a animação
    private bool hasExploded = false; // Flag para evitar múltiplas explosões

    [Header("Áudio")]
    public AudioSource audioSource;  // Fonte de som da explosão
    public AudioClip explodeClip;    // Som da explosão

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") || 
             collision.gameObject.CompareTag("Enemy") || 
             collision.gameObject.CompareTag("Item")) && !hasExploded)
        {
            hasExploded = true;

            // Se for player, aplica dano e knockback
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
                PlayerKnockBack knockback = collision.gameObject.GetComponent<PlayerKnockBack>();

                if (player != null && !player.isInvincible)
                {
                    player.TakeDamage(damage);

                    if (knockback != null)
                    {
                        Vector2 direction = (collision.transform.position - transform.position).normalized;
                        knockback.ApplyKnockback(direction); 
                    }
                }
            }
            // Se for enemy, aplica dano e knockback
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                EnemyKnockBack knockback = collision.gameObject.GetComponent<EnemyKnockBack>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);

                    if (knockback != null)
                    {
                        Vector2 direction = (collision.transform.position - transform.position).normalized;
                        knockback.ApplyKnockback(direction); 
                    }
                }
            }

            // Toca a animação de explosão
            animator.SetTrigger("Explode");

            // Toca o som de explosão
            PlayExplosionSound();

            // Destroi o objeto após 1 segundo 
            Destroy(gameObject, 1f);
        }
    }

    private void PlayExplosionSound()
    {
        if (audioSource != null && explodeClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // pitch levemente aleatório
            audioSource.PlayOneShot(explodeClip);
        }
    }
}
