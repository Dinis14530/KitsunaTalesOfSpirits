using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float currentHealth;
    public HealthDisplay healthDisplay;
    public bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;       // Cor do flash
    public float flashDuration = 0.1f;       // Duração de cada flash
    

    private Vector3 checkpointPosition; // posição do checkpoint

    void Start()
    {
        currentHealth = maxHealth;
        checkpointPosition = transform.position; // checkpoint inicial
        healthDisplay.UpdateHealth(currentHealth);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
        Debug.Log("Checkpoint guardado em: " + checkpointPosition);
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        healthDisplay.UpdateHealth(currentHealth);

        if (currentHealth == 0)
        {
            Die();
        }
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashCoroutine());
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died");

        transform.position = checkpointPosition; 
        currentHealth = maxHealth;
        healthDisplay.UpdateHealth(currentHealth);
    }

    private IEnumerator FlashCoroutine()
    {
        Color originalColor = spriteRenderer.color;

        // Faz o sprite ficar vermelho
        spriteRenderer.color = Color.Lerp(originalColor, Color.red, 0.7f); // Lerp

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
    }
}
