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

    [Header("Death")]
    public GameObject objectToDisappear;

    private int currentColumn;
    private bool isAttacking;
    private bool isActive;
    private Coroutine bossLoopCoroutine;

    IEnumerator BossLoop()
    {
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
            return;

        currentColumn = Random.Range(0, columns.Length);
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

            Vector2 dir = (player.position - shootPoint.position).normalized;

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = dir * 8f;

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
        if (enemyPrefab == null || summonPoints == null)
        {
            isAttacking = false;
            yield break;
        }

        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (summonPoints[i] == null)
                continue;

            Instantiate(enemyPrefab, summonPoints[i].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    IEnumerator DashAttack()
    {
        if (player == null)
        {
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

    public void StartBossFight()
    {
        if (isActive)
            return;

        isActive = true;

        if (lifeBarUI != null)
        {
            lifeBarUI.SetActive(true);

            CanvasGroup lifeBarCanvasGroup =
                lifeBarUI.GetComponent<CanvasGroup>();

            if (lifeBarCanvasGroup != null)
                lifeBarCanvasGroup.alpha = 1f;
        }

        bossLoopCoroutine = StartCoroutine(BossLoop());
    }

    public void Die()
    {
        isActive = false;

        if (bossLoopCoroutine != null)
            StopCoroutine(bossLoopCoroutine);

        if (objectToDisappear != null)
            Destroy(objectToDisappear);

        Destroy(gameObject);
    }

    void OnDisable()
    {
        if (bossLoopCoroutine != null)
            StopCoroutine(bossLoopCoroutine);
    }
}