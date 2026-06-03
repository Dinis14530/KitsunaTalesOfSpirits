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
        for (int i = 0; i < 3; i++)
        {
            GameObject proj = Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );

            Vector2 dir = (player.position - shootPoint.position).normalized;
            proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 8f;

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
        for (int i = 0; i < summonPoints.Length; i++)
        {
            Instantiate(enemyPrefab, summonPoints[i].position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    IEnumerator DashAttack()
    {
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
