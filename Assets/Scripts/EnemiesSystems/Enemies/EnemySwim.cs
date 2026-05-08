using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemySwim : MonoBehaviour
{
    [Header("Swim Settings")]
    public float swimSpeed = 2f;
    public float verticalSwimSpeed = 2f;
    public float waterGravityScale = 0.2f;

    [Header("Status")]
    public bool isInWater;
    public bool isSwimming;

    private Rigidbody2D rb;
    private float originalGravity;
    private Transform player;
    private SpriteRenderer sr;
    private EnemyKnockBack knockback;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        knockback = GetComponent<EnemyKnockBack>();

        originalGravity = rb.gravityScale;
        rb.gravityScale = waterGravityScale;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        isInWater = true;
    }

    private void Update()
    {
        HandleSwimming();
    }

    private void HandleSwimming()
    {
        if (knockback != null && knockback.isKnockback) return;

        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(
            direction.x * swimSpeed,
            direction.y * verticalSwimSpeed
        );

        isSwimming = rb.linearVelocity.magnitude > 0.1f;

        // Rotação
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 90f || angle < -90f)
        {
            sr.flipX = true;
            angle -= 180f;
        }
        else
        {
            sr.flipX = false;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    
}

