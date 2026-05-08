using UnityEngine; 
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]

public class PlayerController : MonoBehaviour
{
    // CONFIGURAÇÃO DE MOVIMENTO
    [Header("Movimento")]
    public float speed = 3f; // Velocidade de movimento 
    public float jumpForce = 7f; // Força vertical aplicada ao saltar

    // COMPONENTES
    private Rigidbody2D rb; // Rigidbody2D 
    private Animator animator; // Animator 
    private SpriteRenderer spriteRenderer; // SpriteRenderer 

    // STATUS DO JOGADOR
    public bool isGrounded; // True se o jogador estiver tocando o chão
    public bool isInDialogue = false; // True se o jogador estiver interagindo com NPC
    private bool isCrouching; // True se o jogador estiver agachado
    public Vector2 lastMoveDirection; // Ultima direção horizontal 
    public bool isDashing; // True se o jogador estiver em dash
    public bool canMove = true; // True se o jogador pode se mover
    private bool isSleeping = true; // True se o jogador está dormindo 

    // ESTA NO CHAO?
    [Header("Ground Check")]
    public Transform groundCheck; // Ponto de checagem do chão
    public float groundCheckRadius = 0.12f; // Raio da checagem
    public LayerMask groundLayer; // Layer chão

    // SALTO
    [Header("Jump")]
    public float coyoteTime = 0.12f; // Tempo que jogador ainda pode saltar após sair do chão
    public float jumpBufferTime = 0.12f; // Tempo que input do salto é lembrado
    public float shortHopMultiplier = 0.5f; // Multiplicador para reduzir altura do salto se soltar W cedo

    // ÁUDIO - FOOTSTEPS
    [Header("Áudio - Footsteps")]
    public AudioSource footstepSource; // Fonte de som para passos
    public AudioClip[] footstepClips;  // Array de sons de passos
    public float minStepInterval = 0.3f;  // Intervalo mínimo entre passos
    
    // public float maxStepInterval = 0.5f;  

    // ÁUDIO - SALTO
    [Header("Áudio - Jump")]
    public AudioSource jumpSource; // Fonte de som do salto
    public AudioClip[] jumpClips; // Array de sons de salto

    // VARIÁVEIS
    private float coyoteTimer = 0f; // Contador coyote time
    private float jumpBufferTimer = 0f;  // Contador jump buffer
    private float baseSpeed;  // Velocidade base original
    private float stepTimer;  // Contador intervalo entre passos
    private System.Collections.IEnumerator speedRoutineCoroutine; // Coroutine buff velocidade

    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;

    // AWAKE 
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 
        animator = GetComponent<Animator>();  // Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRendere GameObject

        animator.applyRootMotion = false; // Root motion false

        // Configura Footsteps
        footstepSource = GetComponent<AudioSource>(); // Pega AudioSource 
        footstepSource.playOnAwake = false; // Não tocar automaticamente
        footstepSource.loop = false;  // Não repetir automaticamente
        footstepSource.spatialBlend = 0f; // Som 

        // Configura Jump
        jumpSource = gameObject.AddComponent<AudioSource>(); // Cria AudioSource 
        jumpSource.playOnAwake = false; // Não tocar automaticamente
        jumpSource.loop = false; // Não repetir
        jumpSource.spatialBlend = 0f; // Som 

        baseSpeed = speed; // Guarda velocidade original
    }

    // START 
    private void Start()
    {
        if (animator != null)
            animator.SetBool("IsSleeping", true); // Coloca jogador no estado dormindo no Animator
        canMove = false; // Inicialmente, jogador não pode se mover
    }

    // FIXEDUPDATE 
    private void FixedUpdate()
    {
        // Checa se jogador está no chão usando OverlapCircle
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position, // posição do GroundCheck
                groundCheckRadius, // raio do círculo
                groundLayer // layer que representa chão
            );

        // Atualiza coyoteTimer
        if (isGrounded) // Se esta no chão, reseta coyote timer
            coyoteTimer = coyoteTime;
        else // Se esta no ar, diminui com o tempo
            coyoteTimer -= Time.fixedDeltaTime;
    }

    // UPDATE -> INPUT E LÓGICA
    private void Update()
    {
        // Atualiza jump buffer
        if (jumpAction.action.WasPressedThisFrame())
            jumpBufferTimer = jumpBufferTime; // Reseta buffer ao apertar W
        else
            jumpBufferTimer -= Time.deltaTime; // Diminui buffer ao longo do tempo

        // Se em diálogo, trava movimento horizontal
        if (isInDialogue)
        {
            ForceIdle();
            return;
        }

        // Se não pode se mover, trava horizontal
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return; // Sai do Update
        }

        // Despertar jogador dormindo
        if (isSleeping)
        {
        if (moveAction.action.ReadValue<Vector2>() != Vector2.zero ||
            jumpAction.action.WasPressedThisFrame())
            {
                isSleeping = false; // Sai do estado de dormir
                if (animator != null)
                    animator.SetBool("IsSleeping", false); // Atualiza Animator
                canMove = true; // Permite movimentar
            }
            else return; // Se nenhuma tecla, sai do Update
        }

        // Movimento e salto
        if (!isDashing) // Se não estiver em dash
        {
            HandleMovement(); // Movimenta horizontal
            HandleJump(); // Verifica e aplica salto
        }

        HandleAnimations(); // Atualiza animações
        HandleFootsteps(); // Toca sons de passos
    }

    // HANDLEMOVEMENT -> MOVIMENTO HORIZONTAL
    private void HandleMovement()
    {
        float xInput = moveAction.action.ReadValue<Vector2>().x; // Input horizontal A/D ou seta

        if (xInput != 0)
            lastMoveDirection = new Vector2(xInput, 0).normalized; // Atualiza direção

        float finalSpeed = isCrouching ? speed * 0.5f : speed; // Reduz velocidade se agachado

        rb.linearVelocity = new Vector2(xInput * finalSpeed, rb.linearVelocity.y); // Aplica velocidade

        // Flip do sprite
        if (xInput > 0) spriteRenderer.flipX = false; // Direita
        else if (xInput < 0) spriteRenderer.flipX = true; // Esquerda
    }

    // HANDLEJUMP -> SALTO
    private void HandleJump()
    {
        if (isCrouching) return; // Não salta agachado

        // Se dentro do jump buffer e coyote time
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Aplica salto
            jumpBufferTimer = 0f; // Zera buffer
            coyoteTimer = 0f; // Zera coyote timer

            PlayRandomJumpSound(); // Toca som de salto aleatório
        }

        // Short hop -> soltar tecla W reduz altura
        if (jumpAction.action.WasReleasedThisFrame() && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * shortHopMultiplier);
        }
    }

    // PlayRandomJumpSound -> som de salto aleatório
    private void PlayRandomJumpSound()
    {
        if (jumpClips == null || jumpClips.Length == 0 || jumpSource == null) return; // Não faz nada se não tiver sons

        int index = Random.Range(0, jumpClips.Length); // Escolhe índice aleatório
        jumpSource.pitch = Random.Range(0.95f, 1.05f); // Pitch aleatório
        jumpSource.PlayOneShot(jumpClips[index]); // Toca som
    }

    // HANDLEANIMATIONS -> animações
    private void HandleAnimations()
    {
        if (isSleeping) return; // Não muda animações se dormindo

        float xVel = Mathf.Abs(rb.linearVelocity.x); // Velocidade horizontal absoluta
        bool isRunning = xVel > 0.1f; // Considera correndo se >0.1

        if (animator != null)
        {
            animator.SetBool("IsRunning", isRunning && !isCrouching); // Rodar animação de correr
            animator.SetBool("IsJumping", !isGrounded); // Rodar animação de saltar

            // Agachar
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

            bool isIdle = xVel < 0.1f && isGrounded && !isCrouching; // Idle se parado
            animator.SetBool("IsIdle", isIdle);
        }
    }

    // HANDLEFOOTSTEPS -> passos aleatórios
    private void HandleFootsteps()
    {
        bool shouldPlayFootsteps =
            Mathf.Abs(rb.linearVelocity.x) > 0.1f && // Se se move horizontalmente
            isGrounded && // No chão
            !isSleeping && // Não dormindo
            canMove && // Pode se mover
            !isInDialogue; // Não em diálogo

        if (!shouldPlayFootsteps)
        {
            stepTimer = 0f; // Zera timer
            return;         
        }

        stepTimer -= Time.deltaTime; // Diminui contador

        if (stepTimer <= 0f)
        {
            PlayRandomFootstep(); // Toca som de passo
            stepTimer = minStepInterval; // Reseta timer
        }
    }

    // PlayRandomFootstep -> passos aleatórios
    private void PlayRandomFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return; // Não faz nada se não tiver sons

        int index = Random.Range(0, footstepClips.Length); // Escolhe aleatório
        footstepSource.pitch = Random.Range(0.95f, 1.05f); // Pitch aleatório
        footstepSource.PlayOneShot(footstepClips[index]); // Toca som
    }

    // OnDrawGizmosSelected
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) // Só se tiver referência
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // Desenha esfera
        }
    }

    // SPEED BOOST
    public void ApplySpeedIncrease(float amount, float duration)
    {
        if (speedRoutineCoroutine != null)
            StopCoroutine(speedRoutineCoroutine); // Para rotina anterior se existir

        speedRoutineCoroutine = SpeedIncreaseCoroutine(amount, duration); // Inicia coroutine
        StartCoroutine(speedRoutineCoroutine);
    }

    public void ForceIdle()
    {
        if (animator == null) return;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        animator.SetBool("IsRunning", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsCrouching", false);
        animator.SetBool("IsIdle", true);
    }

    private System.Collections.IEnumerator SpeedIncreaseCoroutine(float amount, float duration)
    {
        speed = baseSpeed + amount; // Aumenta velocidade
        yield return new WaitForSeconds(duration); // Espera duração
        speed = baseSpeed; // Reseta velocidade
        speedRoutineCoroutine = null; // Limpa referência
    }
    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        crouchAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        crouchAction.action.Disable();
    }
}
