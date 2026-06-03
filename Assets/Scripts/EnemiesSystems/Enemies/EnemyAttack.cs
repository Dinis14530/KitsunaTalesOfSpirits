using UnityEngine;

// Ataque Inimigo
public class EnemyAttack : MonoBehaviour
{
    public int damage = 1; // Dano
    public float cooldownTime = 2f; // Tempo de cooldown entre ataques

    private float lastAttackTime;

    private void OnCollisionEnter2D(Collision2D collision) // Chama sempre que inimigo colide com o player
    {
        if (collision.gameObject.CompareTag("Player") && Time.time - lastAttackTime > cooldownTime)
        {
            if (CombatUtils.TryDamagePlayer(collision.gameObject, damage, transform.position))
            {
                // Reinicia o cooldown apenas depois de um acerto valido
                lastAttackTime = Time.time;
            }
        }
    }
}
