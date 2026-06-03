using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(damage);
        }
    }
}
