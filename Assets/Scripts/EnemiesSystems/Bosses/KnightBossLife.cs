using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Vida e Dano")]
    public int health = 20;
    public Color hitColor = Color.red;
    public float flashDuration = 0.1f;

    [HideInInspector]
    public int healthMax;
    public string BossID { get; private set; }

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

        // Gera ID único baseado no nome e posição
        BossID = gameObject.name + "_" + transform.position.ToString();
        Debug.Log($"Boss criado com ID: {BossID}");

        // Esconde o boss até confirmar que não foi derrotado (evita flash visual)
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
    }

    IEnumerator Start()
    {
        // Espera 1 frame para o PlayerSave ter aplicado os dados do save ao BossManager
        yield return null;

        if (BossManager.Instance != null && BossManager.Instance.IsBossDefeated(BossID))
        {
            Debug.Log($"Boss {BossID} já foi derrotado, destruindo");
            Destroy(gameObject);
            yield break;
        }

        // Boss não foi derrotado — restaurar vida guardada
        if (BossManager.Instance != null)
        {
            int savedHealth = BossManager.Instance.GetBossHealth(BossID);
            if (savedHealth > 0)
            {
                health = savedHealth;
                Debug.Log($"Boss {BossID} vida restaurada: {health}");
            }
        }

        // Mostrar
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
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

        // Atualiza a vida no BossManager para ser gravada no save
        if (BossManager.Instance != null)
            BossManager.Instance.SetBossHealth(BossID, health);

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

        // Marca este boss como derrotado e limpa a vida guardada
        if (BossManager.Instance != null)
        {
            BossManager.Instance.MarkBossAsDefeated(BossID);
            BossManager.Instance.SetBossHealth(BossID, 0);
        }

        DropLoot();
        Destroy(gameObject);
    }

    void DropLoot()
    {
        if (lootDrops == null || lootDrops.Length == 0)
            return;

        foreach (LootDrop loot in lootDrops)
        {
            if (loot.itemSO == null)
                continue;

            int randomChance = Random.Range(0, 101);
            if (randomChance <= loot.dropChance)
            {
                LootHelper.SpawnLootItem(loot.itemSO, transform.position, loot.quantity);
                Debug.Log($"{loot.itemSO.itemName} dropped");
            }
        }
    }
}
