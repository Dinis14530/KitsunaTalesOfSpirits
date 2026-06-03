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
    public int damage;
    public int health;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Reinicia o tempo sempre que o projecil volta da pool
        timer = 0f;

        if (player != null && rb != null)
        {
            // Aponta o projecil para a posicao actual do player
            Vector3 direction = player.transform.position - transform.position;
            rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > bulletTimeInScreen)
        {
            ObjectPoolManager.Release(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CombatUtils.TryDamagePlayer(other.gameObject, damage, transform.position);
            // O projecil sai de cena assim que acerta ou testa colisao com o player
            ObjectPoolManager.Release(gameObject);
        }
    }
}
