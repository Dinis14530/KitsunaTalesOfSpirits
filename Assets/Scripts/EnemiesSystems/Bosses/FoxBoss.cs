using System.Collections;
using UnityEngine;

public class FoxBoss : MonoBehaviour
{
    [Header("Columns")]
    public Transform[] columns;

    [Header("Player")]
    public Transform player;

    [Header("Settings")]
    public float waitBeforeAttack = 1f;
    public float waitAfterAttack = 2f;

    [Header("Projectiles")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Summon")]
    public GameObject enemyPrefab;
    public Transform[] summonPoints;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.4f;

    [Header("UI")]
    public GameObject lifeBarUI;

    private int currentColumn;
    private bool isAttacking;
    private bool isActive;
    private Coroutine bossLoopCoroutine;

    IEnumerator BossLoop()
    {
        Debug.Log("Boss iniciado!");

        while (isActive)
        {
            if (!isAttacking)
            {
                TeleportToRandomColumn();

                yield return new WaitForSeconds(waitBeforeAttack);

                PerformAttack();

                yield return new WaitForSeconds(waitAfterAttack);
            }

            yield return null;
        }
    }

    void TeleportToRandomColumn()
    {
        if (columns == null || columns.Length == 0)
        {
            Debug.LogWarning("Nenhuma coluna atribuída ao boss!");
            return;
        }

        currentColumn = Random.Range(0, columns.Length);
        transform.position = columns[currentColumn].position;

        Debug.Log($"Boss teleportou para a coluna {currentColumn + 1}");
    }

    void PerformAttack()
    {
        isAttacking = true;

        switch (currentColumn)
        {
            case 0:
                Debug.Log("Boss usou ATAQUE DE PROJÉTEIS");
                StartCoroutine(ProjectileAttack());
                break;

            case 1:
                Debug.Log("Boss usou ONDA DE CHOQUE");
                StartCoroutine(ShockwaveAttack());
                break;

            case 2:
                Debug.Log("Boss invocou INIMIGOS");
                StartCoroutine(SummonAttack());
                break;

            case 3:
                Debug.Log("Boss fez DASH");
                StartCoroutine(DashAttack());
                break;

            default:
                Debug.LogWarning("Coluna sem ataque definido!");
                isAttacking = false;
                break;
        }
    }

    IEnumerator ProjectileAttack()
    {
        if (projectilePrefab == null || shootPoint == null || player == null)
        {
            Debug.LogWarning("Faltam referências para o ataque de projéteis!");
            isAttacking = false;
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"Projétil {i + 1} disparado");

            GameObject proj = Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );

            Vector2 dir = (player.position - shootPoint.position).normalized;

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = dir * 8f;

            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("Ataque de projéteis terminado");

        isAttacking = false;
    }

    IEnumerator ShockwaveAttack()
    {
        Debug.Log("Preparando onda de choque...");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("SHOCKWAVE!");

        yield return new WaitForSeconds(1f);

        Debug.Log("Onda de choque terminada");

        isAttacking = false;
    }

    IEnumerator SummonAttack()
    {
        if (enemyPrefab == null || summonPoints == null)
        {
            Debug.LogWarning("Faltam referências para a invocação!");
            isAttacking = false;
            yield break;
        }

        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (summonPoints[i] == null)
                continue;

            Instantiate(enemyPrefab, summonPoints[i].position, Quaternion.identity);

            Debug.Log($"Inimigo invocado no ponto {i + 1}");
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("Invocação terminada");

        isAttacking = false;
    }

    IEnumerator DashAttack()
    {
        if (player == null)
        {
            Debug.LogWarning("Jogador não atribuído!");
            isAttacking = false;
            yield break;
        }

        Debug.Log("Boss iniciou DASH");

        Vector2 start = transform.position;
        Vector2 target = player.position;

        float time = 0f;

        while (time < dashDuration)
        {
            transform.position = Vector2.Lerp(start, target, time / dashDuration);

            time += Time.deltaTime;

            yield return null;
        }

        transform.position = target;

        Debug.Log("Boss terminou DASH");

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    public void StartBossFight()
    {
        if (isActive)
            return;

        Debug.Log("Luta contra o boss começou!");

        isActive = true;

        if (lifeBarUI != null)
        {
            lifeBarUI.SetActive(true);

            CanvasGroup canvasGroup = lifeBarUI.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }

        bossLoopCoroutine = StartCoroutine(BossLoop());
    }

    void OnDisable()
    {
        if (bossLoopCoroutine != null)
            StopCoroutine(bossLoopCoroutine);

        Debug.Log("Boss desativado");
    }
}
