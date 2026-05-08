using UnityEngine;
using System.Collections;

// Vida e Drop de itens Inimigo
public class Enemy : MonoBehaviour
{
    public int health = 5; // Vida do inimigo
    private SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;       // Cor do flash
    public float flashDuration = 0.1f;       // Duração de cada flash
    
    [System.Serializable]
    public class LootDrop
    {
        public ItemSO itemSO;       // ScriptableObject do item
        [Range(0, 100)]
        public int dropChance;      // Chance de drop
        public int quantity = 1;    // Quantidade a dropar
    }

    public LootDrop[] lootDrops;   // Array de possíveis loots

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
            StartCoroutine(FlashCoroutine());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashCoroutine()
    {
        Color originalColor = spriteRenderer.color;

        // Faz o sprite ficar vermelho
        spriteRenderer.color = Color.Lerp(originalColor, Color.red, 0.7f); // Lerp

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died");

        DropLoot();

        Destroy(gameObject);
    }

    void DropLoot()
    {
        if (lootDrops == null || lootDrops.Length == 0)
            return;

        foreach (LootDrop loot in lootDrops)
        {
            if (loot.itemSO == null) continue;

            int randomChance = Random.Range(0, 101); // 0-100
            if (randomChance <= loot.dropChance)
            {
                LootHelper.SpawnLootItem(loot.itemSO, transform.position, loot.quantity);
                Debug.Log($"{loot.itemSO.itemName} dropped");
            }
        }
    }
}
