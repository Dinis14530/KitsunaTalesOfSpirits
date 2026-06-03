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

    public LootDrop[] lootDrops;

    private SpriteRenderer spriteRenderer;
    private BossController bossController;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossController = GetComponent<BossController>();
        healthMax = health;

        // Gera ID unico baseado no nome e posicao
        BossID = gameObject.name + "_" + transform.position.ToString();
        Debug.Log($"Boss criado com ID: {BossID}");
    }

    void Start()
    {
        // Verifica se este boss ja foi derrotado
        if (BossManager.Instance != null && BossManager.Instance.IsTracked(BossID))
        {
            Debug.Log($"Boss {BossID} ja foi derrotado, destruindo");
            Destroy(gameObject);
            return;
        }
    }

    public void TakeDamage(int damage)
    {
        // So recebe dano quando esta no chao
        if (bossController != null && bossController.state != BossState.Grounded)
        {
            Debug.Log("Boss is immune to damage while flying.");
            return;
        }

        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + health);

        if (spriteRenderer != null)
            StartCoroutine(SpriteFlash.Flash(spriteRenderer, hitColor, flashDuration));

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died");

        // Marca este boss como derrotado
        if (BossManager.Instance != null)
            BossManager.Instance.MarkTracked(BossID);

        LootTable.TryDrop(lootDrops, transform.position);
        Destroy(gameObject);
    }
}
