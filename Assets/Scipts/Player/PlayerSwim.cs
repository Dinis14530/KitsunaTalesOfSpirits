using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSwim : MonoBehaviour
{
    [Header("Swim Settings")]
    public float swimSpeed = 2f;
    public float verticalSwimSpeed = 2f;
    public float waterGravityScale = 0.2f;

    [Header("Breathing System")]
    public float maxBreathTime = 10f; // Tempo máximo na água antes de começar a se afogar
    public float currentBreathTime;

    [Header("Status")]
    public bool isInWater;
    public bool isSwimming;

    private Rigidbody2D rb;
    private float originalGravity;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private float damageTimer;
    private bool isDrowning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
        originalGravity = rb.gravityScale;
    }

    private void Update()
    {
        if (!isInWater) return;

        HandleSwimming();
        HandleBreathing();
    }

    private void HandleSwimming()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = 0f;

        if (Input.GetKey(KeyCode.W))
            yInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            yInput = -1f;

        rb.linearVelocity = new Vector2(
            xInput * swimSpeed,
            yInput * verticalSwimSpeed
        );

        isSwimming = Mathf.Abs(xInput) > 0.1f || Mathf.Abs(yInput) > 0.1f;
    }

    private void HandleBreathing()
    {
        if (currentBreathTime > 0)
        {
            currentBreathTime -= Time.deltaTime;
        }
        else if (!isDrowning)
        {
            isDrowning = true;
            damageTimer = 0f;
        }

        if (isDrowning)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 2f)
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
                damageTimer = 0f;
            }
        }
    }

    private void EnterWater()
    {
        isInWater = true;
        rb.gravityScale = waterGravityScale;
        currentBreathTime = maxBreathTime;
        isDrowning = false;
        damageTimer = 0f;

        if (playerController != null)
        {
            playerController.isGrounded = false;
        }
    }

    private void ExitWater()
    {
        isInWater = false;
        isSwimming = false;
        rb.gravityScale = originalGravity;
        isDrowning = false;
        currentBreathTime = maxBreathTime;
        damageTimer = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            EnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            ExitWater();
        }
    }
}
