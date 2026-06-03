using UnityEngine;

// Vida e Drop de itens Inimigo
public class Enemy : MonoBehaviour
{
    public int health = 5; // Vida do inimigo
    private SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red; // Cor do flash
    public float flashDuration = 0.1f; // Duração de cada flash

    public LootDrop[] lootDrops; // Array de possíveis loots

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + health);

        // Pisca quando leva dano
        if (spriteRenderer != null)
        {
            StartCoroutine(SpriteFlash.Flash(spriteRenderer, hitColor, flashDuration));
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died");

        LootTable.TryDrop(lootDrops, transform.position);

        Destroy(gameObject);
    }
}
