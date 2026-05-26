using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]
public class PlayerDash : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    private Animator anim;
    private PlayerHealth playerHealth;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.5f;

    private float dashCooldownTimer;
    private float dashTimeLeft;
    public bool canDash = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        // O dash so dispara quando esta habilitado, fora de dash e com cooldown pronto
        if (
            canDash
            && !playerController.isDashing
            && Input.GetKeyDown(KeyCode.R)
            && dashCooldownTimer <= 0
        )
            StartDash();

        if (playerController.isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
                EndDash();
        }
    }

    private void StartDash()
    {
        if (playerController.lastMoveDirection == Vector2.zero)
            playerController.lastMoveDirection = Vector2.right; // Dash para frente se parado

        // Trava o estado do player durante a corrida curta
        playerController.isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Aplica a velocidade do dash na direcao guardada
        rb.linearVelocity = playerController.lastMoveDirection * dashSpeed;

        // Invencibilidade durante o dash
        playerHealth.isInvincible = true;

        // Ignora colisao com a layer usada para itens/obstaculos desse estado
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Items"), true);

        if (anim != null)
            anim.SetBool("IsDashing", true);
    }

    private void EndDash()
    {
        // Reverte o estado de dash e preserva a velocidade vertical existente
        playerController.isDashing = false;

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Remove a invencibilidade ao terminar o impulso
        if (playerHealth != null)
            playerHealth.isInvincible = false;

        // Reativa a colisao normal
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Items"), false);

        if (anim != null)
            anim.SetBool("IsDashing", false);
    }
}
