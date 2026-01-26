using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5;
    public float currentHealth;
    public HealthDisplay healthDisplay;
    public bool isInvincible = false;

    private Vector3 checkpointPosition; // posição do checkpoint

    void Start()
    {
        currentHealth = maxHealth;
        checkpointPosition = transform.position; // checkpoint inicial
        healthDisplay.UpdateHealth(currentHealth);
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
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died");

        transform.position = checkpointPosition; 
        currentHealth = maxHealth;
        healthDisplay.UpdateHealth(currentHealth);
    }
}
