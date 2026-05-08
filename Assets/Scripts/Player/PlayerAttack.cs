using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    public bool isAttacking = false;
    private float timeBtwToAttack;
    public float startTimeBtwAttack = 0.5f;
    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRange = 0.5f;
    public int baseDamage = 1;
    private float damageMultiplier = 1f;
    private Coroutine strengthRoutine;
    public AudioClip attackSound;
    private AudioSource audioSource;

    [Header("Input System")]
    [SerializeField] private InputActionReference attackAction;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (timeBtwToAttack > 0)
            timeBtwToAttack -= Time.deltaTime;

        if (timeBtwToAttack <= 0)
        {
            if (attackAction.action.WasPressedThisFrame())
            {
                anim.SetTrigger("Attack");
                timeBtwToAttack = startTimeBtwAttack;

                if (attackSound != null)
                    audioSource.PlayOneShot(attackSound);
            }

        }
    }

    // Chamado pela animação de ataque 
    public void Attack()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);

        int currentDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

        foreach (Collider2D col in enemies)
        {
            // Inimigos normais
            Enemy enemy = col.GetComponent<Enemy>();
            EnemyKnockBack knock = col.GetComponent<EnemyKnockBack>();
            if (enemy != null)
            {
                enemy.TakeDamage(currentDamage);

                if (knock != null)
                {
                    Vector2 direction = (col.transform.position - transform.position).normalized;
                    knock.ApplyKnockback(direction);
                }
            }

            // Boss
            BossHealth boss = col.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(currentDamage);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    // Aplica multiplicador temporário 
    public void ApplyStrengthMultiplier(float multiplier, float duration)
    {
        if (strengthRoutine != null)
            StopCoroutine(strengthRoutine);

        strengthRoutine = StartCoroutine(StrengthCoroutine(multiplier, duration));
    }

    private IEnumerator StrengthCoroutine(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        damageMultiplier = 1f;
        strengthRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPos == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
