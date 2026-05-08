using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyKnockBack : MonoBehaviour
{
    public float KBforceX = 6f;
    public float KBforceY = 3f;
    public float KBTime = 0.15f;

    [HideInInspector] public bool isKnockback = false;

    private float counter;
    private Vector2 knockbackVelocity;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isKnockback) return;

        counter -= Time.deltaTime;

        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            // knockback manual
            rb.MovePosition(
                rb.position + knockbackVelocity * Time.deltaTime
            );
        }

        if (counter <= 0)
        {
            isKnockback = false;
            knockbackVelocity = Vector2.zero;
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        isKnockback = true;
        counter = KBTime;

        direction.Normalize();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            // knockback físico
            rb.linearVelocity = Vector2.zero;

            Vector2 force = new Vector2(
                direction.x * KBforceX,
                KBforceY
            );

            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            // knockback manual (kinematic)
            knockbackVelocity = new Vector2(
                direction.x * KBforceX,
                KBforceY
            );
        }
    }
}
