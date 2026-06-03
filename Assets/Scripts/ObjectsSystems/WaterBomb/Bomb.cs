using UnityEngine;

public class Bomb : MonoBehaviour
{
    public int damage = 2; // Dano
    public Animator animator; // Animator para a animacao
    private bool hasExploded = false; // Flag para evitar multiplas explosoes

    [Header("Audio")]
    public AudioSource audioSource; // Fonte de som da explosao
    public AudioClip explodeClip; // Som da explosao

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (
            (
                collision.gameObject.CompareTag("Player")
                || collision.gameObject.CompareTag("Enemy")
                || collision.gameObject.CompareTag("Item")
            ) && !hasExploded
        )
        {
            hasExploded = true;

            // Se for player, aplica dano e knockback
            if (collision.gameObject.CompareTag("Player"))
            {
                CombatUtils.TryDamagePlayer(collision.gameObject, damage, transform.position);
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
                        Vector2 direction = (
                            collision.transform.position - transform.position
                        ).normalized;
                        knockback.ApplyKnockback(direction);
                    }
                }
            }

            // Toca a animacao de explosao
            animator.SetTrigger("Explode");

            // Toca o som de explosao
            AudioHelper.PlayWithRandomPitch(audioSource, explodeClip);

            // Destroi o objeto apos 1 segundo
            Destroy(gameObject, 1f);
        }
    }
}
