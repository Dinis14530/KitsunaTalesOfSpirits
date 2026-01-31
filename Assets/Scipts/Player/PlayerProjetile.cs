using UnityEngine;

public class Projetile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 2f;

    private int damage;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<Enemy>()?.TakeDamage(damage);
            col.GetComponent<BossHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (col.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
