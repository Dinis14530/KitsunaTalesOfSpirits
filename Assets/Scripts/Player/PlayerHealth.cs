using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float currentHealth;
    public HealthDisplay healthDisplay;
    public bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red; // Cor do flash
    public float flashDuration = 0.1f; // Duracao de cada flash

    private Vector3 checkpointPosition; // posicao do checkpoint

    [Header("Audio")]
    public AudioSource audioSource; // Fonte de som de dano
    public AudioClip damageClip; // Som de dano

    void Start()
    {
        currentHealth = maxHealth;
        // O checkpoint inicial e a posicao actual do player no arranque
        checkpointPosition = transform.position; // checkpoint inicial
        healthDisplay.UpdateHealth(currentHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
        Debug.Log("Checkpoint guardado em: " + checkpointPosition);
    }

    public void RestoreHealth(float health)
    {
        currentHealth = health;
        healthDisplay.UpdateHealth(currentHealth);
    }

    public void TakeDamage(float amount)
    {
        // Bloqueia dano enquanto durar a invencibilidade
        if (isInvincible)
            return;

        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;

        healthDisplay.UpdateHealth(currentHealth);

        // Toca som de dano
        AudioHelper.PlayWithRandomPitch(audioSource, damageClip);

        if (currentHealth == 0)
        {
            Die();
        }
        if (spriteRenderer != null)
        {
            // Flash visual apenas quando existe sprite para animar
            StartCoroutine(SpriteFlash.Flash(spriteRenderer, hitColor, flashDuration));
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died");

        // Volta ao ultimo checkpoint em vez de destruir o player
        transform.position = checkpointPosition;
        currentHealth = maxHealth;
        healthDisplay.UpdateHealth(currentHealth);
    }
}
