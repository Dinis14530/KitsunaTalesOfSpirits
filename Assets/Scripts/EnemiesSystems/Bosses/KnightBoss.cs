using UnityEngine;
using System.Collections;

public enum BossState
{
    Flying,
    Attacking,
    Grounded
}

public class BossController : MonoBehaviour
{
    [Header("Estado do Boss")]
    public BossState state = BossState.Flying;

    [Header("Movimento")]
    public float speed = 3f;
    public float minX;
    public float maxX;

    [Header("Ataque")]
    public GameObject attackHitbox;
    public Transform attackSpawnPoint;
    public int attacksBeforeGround = 3;
    public float flyTimeMin = 1f;
    public float flyTimeMax = 2f;

    [Header("Chão")]
    public float groundY = 0f;
    public float fallSpeed = 5f;
    public float groundStayTime = 3f;

    private Animator anim;
    private bool movingRight = true;
    private float flyY;
    private bool isActive = false;

    [Header("UI")]
    public GameObject lifeBarUI; 

    void Start()
    {
        anim = GetComponent<Animator>();
        flyY = transform.position.y;

        // Começa a voar
        SetAnimation(true, false, false);
    }

    void Update()
    {
        if (!isActive) return;

        if (state == BossState.Flying)
        {
            Move();
        }
        else if (state == BossState.Grounded)
        {
            float step = fallSpeed * Time.deltaTime;
            transform.position = new Vector3(
                transform.position.x,
                Mathf.MoveTowards(transform.position.y, groundY, step),
                transform.position.z
            );
        }
    }

    IEnumerator BossLoop()
    {
        while (true)
        {
            // Ataques enquanto voa
            for (int i = 0; i < attacksBeforeGround; i++)
            {
                state = BossState.Flying;
                SetAnimation(true, false, false);

                float flyTime = Random.Range(flyTimeMin, flyTimeMax);
                yield return new WaitForSeconds(flyTime);

                yield return StartCoroutine(Attack());
            }

            // Descer ao chão
            GoToGround();

            while (transform.position.y > groundY)
                yield return null;

            yield return new WaitForSeconds(groundStayTime);

            // Subir novamente
            state = BossState.Flying;
            SetAnimation(true, false, false);

            float currentX = transform.position.x;
            while (transform.position.y < flyY)
            {
                float step = fallSpeed * Time.deltaTime;
                transform.position = new Vector3(
                    currentX,
                    Mathf.MoveTowards(transform.position.y, flyY, step),
                    transform.position.z
                );
                yield return null;
            }
        }
    }

    void Move()
    {
        float dir = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        if (transform.position.x >= maxX)
            movingRight = false;
        else if (transform.position.x <= minX)
            movingRight = true;
    }

    IEnumerator Attack()
    {
        state = BossState.Attacking;
        SetAnimation(false, true, false);

        yield return new WaitForSeconds(0.5f);

        if (attackHitbox != null && attackSpawnPoint != null)
            Instantiate(attackHitbox, attackSpawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);

        state = BossState.Flying;
        SetAnimation(true, false, false);
    }

    void GoToGround()
    {
        state = BossState.Grounded;
        SetAnimation(false, false, true);
    }

    void SetAnimation(bool isFlying, bool isAttacking, bool isResting)
    {
        anim.SetBool("IsFlying", isFlying);
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("IsResting", isResting);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isActive && col.CompareTag("Player"))
        {
            isActive = true;
            StartCoroutine(BossLoop());
            lifeBarUI.SetActive(true);
        }
    }
}
