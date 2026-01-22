using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Vida e Dano")]
    public int health = 20;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;
    [HideInInspector]
    public int healthMax;

    [System.Serializable]
    public class LootDrop
    {
        public ItemSO itemSO;
        [Range(0, 100)]
        public int dropChance;
        public int quantity = 1;
    }
    public LootDrop[] lootDrops;

    private SpriteRenderer spriteRenderer;
    private BossController bossController;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossController = GetComponent<BossController>();
        healthMax = health;
    }

    public void TakeDamage(int damage)
    {
        // Só recebe dano quando está no chão
        if (bossController != null && bossController.state != BossState.Grounded)
        {
            Debug.Log("Boss is immune to damage while flying.");
            return;
        }

        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + health);

        if (spriteRenderer != null)
            StartCoroutine(FlashCoroutine());

        if (health <= 0)
            Die();
    }

    private IEnumerator FlashCoroutine()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.Lerp(originalColor, hitColor, 0.7f);
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

            int randomChance = Random.Range(0, 101);
            if (randomChance <= loot.dropChance)
            {
                LootHelper.SpawnLootItem(loot.itemSO, transform.position, loot.quantity);
                Debug.Log($"{loot.itemSO.itemName} dropped");
            }
        }
    }
}
