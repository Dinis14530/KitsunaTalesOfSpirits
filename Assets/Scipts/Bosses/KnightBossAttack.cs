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
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>(); 

        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(damage);
        }
    }
}
