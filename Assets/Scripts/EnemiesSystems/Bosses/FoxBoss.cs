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

    private int currentColumn;
    private bool isAttacking;

    void Start()
    {
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (true)
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
            Debug.LogWarning("[FoxBoss] No columns configured for teleport.");
            return;
        }

        currentColumn = Random.Range(0, columns.Length);
        if (columns[currentColumn] == null)
        {
            Debug.LogWarning($"[FoxBoss] Column {currentColumn} is null.");
            return;
        }

        transform.position = columns[currentColumn].position;
    }

    void PerformAttack()
    {
        isAttacking = true;

        switch (currentColumn)
        {
            case 0:
                StartCoroutine(ProjectileAttack());
                break;

            case 1:
                StartCoroutine(ShockwaveAttack());
                break;

            case 2:
                StartCoroutine(SummonAttack());
                break;

            case 3:
                StartCoroutine(DashAttack());
                break;
        }
    }

    IEnumerator ProjectileAttack()
    {
        if (projectilePrefab == null || shootPoint == null || player == null)
        {
            Debug.LogWarning(
                "[FoxBoss] ProjectileAttack missing references (prefab/shootPoint/player)."
            );
            isAttacking = false;
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject proj = Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (player.position - shootPoint.position).normalized;
                rb.linearVelocity = dir * 8f;
            }
            else
            {
                Debug.LogWarning("[FoxBoss] Projectile prefab is missing Rigidbody2D.");
            }

            yield return new WaitForSeconds(0.3f);
        }

        isAttacking = false;
    }

    IEnumerator ShockwaveAttack()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Shockwave!");

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    IEnumerator SummonAttack()
    {
        if (enemyPrefab == null || summonPoints == null || summonPoints.Length == 0)
        {
            Debug.LogWarning(
                "[FoxBoss] SummonAttack missing references (enemyPrefab/summonPoints)."
            );
            isAttacking = false;
            yield break;
        }

        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (summonPoints[i] != null)
                Instantiate(enemyPrefab, summonPoints[i].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    IEnumerator DashAttack()
    {
        if (player == null)
        {
            Debug.LogWarning("[FoxBoss] DashAttack: player reference is null.");
            isAttacking = false;
            yield break;
        }

        Vector2 start = transform.position;
        Vector2 target = player.position;

        float time = 0;

        while (time < dashDuration)
        {
            transform.position = Vector2.Lerp(start, target, time / dashDuration);
            time += Time.deltaTime * dashSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}
