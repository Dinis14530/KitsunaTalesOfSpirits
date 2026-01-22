using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject player;
    private Rigidbody2D rb;
    public float force;
    public float bulletTimeInScreen;
    private float timer;
    public int damage = 1; // Dano
    public int health = 1;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > bulletTimeInScreen)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>(); // Vida do jopgador
            PlayerKnockBack knockback = other.gameObject.GetComponent<PlayerKnockBack>(); // Knockback

            if (player != null && !player.isInvincible) // Se o jogador nao estiver invencivel 
            {
                // Aplica dano
                player.TakeDamage(damage);

                // Aplica knockback no player
                if (knockback != null)
                {
                    // Calcula direção do knockback 
                    Vector2 direction = (other.transform.position - transform.position).normalized;

                    knockback.ApplyKnockback(direction); 
                }
            }
            Destroy(gameObject);
        }
    }
}
