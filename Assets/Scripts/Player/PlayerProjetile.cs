using UnityEngine;

public class Projetile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 2f;

    private int damage;
    private Vector2 direction;
    private float lifeTimer;

    private void OnEnable()
    {
        lifeTimer = 0f;
        damage = 0;
        direction = Vector2.zero;
    }

    public void Initialize(Vector2 dir, int dmg)
    {
        direction = dir.normalized;
        damage = dmg;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            ReleaseProjectile();
            return;
        }

        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<Enemy>()?.TakeDamage(damage);
            col.GetComponent<BossHealth>()?.TakeDamage(damage);
            ReleaseProjectile();
        }

        if (col.CompareTag("Wall"))
        {
            ReleaseProjectile();
        }
    }

    private void ReleaseProjectile()
    {
        ObjectPoolManager.Release(gameObject);
    }
}
