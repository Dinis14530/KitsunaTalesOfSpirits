using UnityEngine;

// Knockback Player 
public class PlayerKnockBack : MonoBehaviour
{
    public float KBforceX = 10f;       // Força horizontal do knockback
    public float KBforceY = 5f;        // Força vertical (salto)
    public float KBTotalTime = 0.2f;   // Duração do knockback
    private float KBCounter;           // Contador interno
    private Rigidbody2D rb;
    private PlayerController movement; // Para desativar o movimento durante knockback

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (KBCounter > 0)
        {
            KBCounter -= Time.deltaTime;

            rb.linearVelocity = new Vector2(KBforceX, KBforceY);

            // Impede o movimento normal do jogador
            if (movement != null)
                movement.canMove = false;
        }
        else
        {
            // Fim do knockback, permite movimento normal
            if (movement != null)
                movement.canMove = true;
        }
    }

    // Método para aplicar knockback
    public void ApplyKnockback(Vector2 direction)
    {
        KBforceX = Mathf.Abs(KBforceX) * Mathf.Sign(direction.x); // Define a direção correta
        KBCounter = KBTotalTime;
    }

    // Detecta colisão com inimigo
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Calcula direção do knockback com base na posição do inimigo
            Vector2 knockDirection = (transform.position - collision.transform.position).normalized;
            ApplyKnockback(knockDirection);
        }
    }
}
