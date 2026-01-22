using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 3f;
    public float jumpForce = 7f;

    [Header("Componentes")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Status")]
    public bool isGrounded;
    public bool isInDialogue = false;
    private bool isCrouching;
    public Vector2 lastMoveDirection;
    public bool isDashing;
    public bool canMove = true;
    private bool isSleeping = true;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    [Header("Jump Tweaks")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    public float shortHopMultiplier = 0.5f;

    // Internals
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private float baseSpeed;
    private System.Collections.IEnumerator speedRoutineCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.applyRootMotion = false;

        baseSpeed = speed;
    }

    private void Start()
    {
        // Começa no estado de dormir
        if (animator != null)
            animator.SetBool("IsSleeping", true);
        canMove = false;
    }

    private void FixedUpdate()
    {
        // Ground check com OverlapCircle 
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Atualiza coyote timer 
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.fixedDeltaTime;
    }

    private void Update()
    {
        // Update timers e input
        if (Input.GetKeyDown(KeyCode.W))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (isInDialogue)
        {
            // trava movimento horizontal enquanto em diálogo
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null)
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsCrouching", false);
                animator.SetBool("IsIdle", true);
            }
            return;
        }

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (animator != null) animator.SetBool("IsRunning", false);
            return;
        }

        if (isSleeping)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                isSleeping = false;
                if (animator != null) animator.SetBool("IsSleeping", false);
                canMove = true;
            }
            else
            {
                return;
            }
        }

        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        HandleAnimations();
    }

    private void HandleMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0)
            lastMoveDirection = new Vector2(xInput, 0).normalized;

        float finalSpeed = isCrouching ? speed * 0.5f : speed;

        rb.linearVelocity = new Vector2(xInput * finalSpeed, rb.linearVelocity.y);

        // Flip do sprite
        if (xInput > 0) spriteRenderer.flipX = false;
        else if (xInput < 0) spriteRenderer.flipX = true;
    }

    private void HandleJump()
    {
        if (isCrouching) return;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            PerformJump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        // Short hop
        if (Input.GetKeyUp(KeyCode.W) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * shortHopMultiplier);
        }
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (animator != null)
            animator.SetBool("IsJumping", true);
    }

    private void HandleAnimations()
    {
        if (isSleeping) return;

        float xVel = Mathf.Abs(rb.linearVelocity.x);
        bool isRunning = xVel > 0.1f;

        if (animator != null)
        {
            animator.SetBool("IsRunning", isRunning && !isCrouching);

            // Crouch
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
            {
                if (!isCrouching)
                {
                    isCrouching = true;
                    animator.SetBool("IsCrouching", true);
                }
            }
            else if (isCrouching)
            {
                isCrouching = false;
                animator.SetBool("IsCrouching", false);
            }

            // Jump
            animator.SetBool("IsJumping", !isGrounded);

            // Idle
            bool isIdle = xVel < 0.1f && isGrounded && !isCrouching;
            animator.SetBool("IsIdle", isIdle);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void ApplySpeedIncrease(float amount, float duration)
    {
        if (speedRoutineCoroutine != null)
            StopCoroutine(speedRoutineCoroutine);

        speedRoutineCoroutine = SpeedIncreaseCoroutine(amount, duration);
        StartCoroutine(speedRoutineCoroutine);
    }

    private System.Collections.IEnumerator SpeedIncreaseCoroutine(float amount, float duration)
    {
        speed = baseSpeed + amount;
        yield return new WaitForSeconds(duration);
        speed = baseSpeed;
        speedRoutineCoroutine = null;
    }
}