using UnityEngine;

// Vida Player
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 5; // Vida maxima
    public float currentHealth; // Vida atual
    public HealthDisplay healthDisplay; // LifeBar
    public bool isInvincible = false; // Flag player invencível

    void Start()
    {
        currentHealth = maxHealth; // Comeca com a vida maxima
        healthDisplay.UpdateHealth(currentHealth); // Display vida
    }
    public void TakeDamage(float amount)
    {
        if (isInvincible) // Se esta invencivel nao toma dano
        {
            return;
        }

        Debug.Log(gameObject.name + " took 1 damage Remaining health: " + currentHealth); // Debug
        currentHealth -= amount;  // Atualiza vida apos tomar dano
        if (currentHealth < 0) currentHealth = 0; // Vida nao pode ser menor que 0

        // Morre
        if (currentHealth == 0) // Se vida for 0
        {
            Die();
        }

        void Die()
        {
            Debug.Log($"{gameObject.name} died");
            transform.position = new  Vector3(0f, 0f, 0f); // Reinicia posição do player
            currentHealth = maxHealth; // Reseta a vida
            healthDisplay.UpdateHealth(currentHealth); // Update na UI
        }

        healthDisplay.UpdateHealth(currentHealth); // Display vida 
    }
}
