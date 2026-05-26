using UnityEngine;
using UnityEngine.InputSystem;

// Sistema de tiro do jogador
public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab do projétil
    public Transform firePoint; // Ponto de onde o projétil sai
    public float fireRate = 0.3f; // Tempo entre cada tiro
    private float fireCooldown; // Cooldown atual do tiro
    public int baseDamage = 1; // Dano base do tiro
    private float damageMultiplier = 1f; // Multiplicador de dano (usado por buffs)
    private bool facingRight = true; // Indica para que lado o jogador está virado

    [Header("Input System")]
    [SerializeField]
    private InputActionReference shootAction;

    void Update()
    {
        // Mantem a direcao de disparo alinhada com o movimento actual
        HandleDirection(); // Atualiza a direção do jogador
        // Reseta o tiro apenas quando o cooldown permite
        HandleCooldown(); // Atualiza o cooldown do tiro

        // Dispara quando a tecla X é pressionada e o cooldown é zero
        if (shootAction.action.WasPressedThisFrame() && fireCooldown <= 0)
        {
            Shoot();
            fireCooldown = fireRate; // reinicia o cooldown
        }
    }

    // Controla a redução do cooldown ao longo do tempo
    void HandleCooldown()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
    }

    // Determina a direção do jogador
    void HandleDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");

        // Guarda o ultimo lado valido para reutilizar no disparo e no pool
        // Se o jogador se move para a direita
        if (h > 0)
            facingRight = true;
        // Se o jogador se move para a esquerda
        else if (h < 0)
            facingRight = false;
    }

    // Dispara o projétil
    void Shoot()
    {
        // Reutiliza o projecil da pool para evitar Instantiate durante combate
        GameObject proj = ObjectPoolManager.Spawn(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );
        if (proj == null)
            return;

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Projetile projScript = proj.GetComponent<Projetile>();

        if (projScript == null)
        {
            Debug.LogWarning("O prefab do projétil do jogador precisa do componente Projetile.");
            // Se o prefab estiver incompleto, devolve logo o objecto a pool
            ObjectPoolManager.Release(proj);
            return;
        }

        projScript.Initialize(dir, Mathf.RoundToInt(baseDamage * damageMultiplier));
    }

    // Buff de dano
    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
    }
}
