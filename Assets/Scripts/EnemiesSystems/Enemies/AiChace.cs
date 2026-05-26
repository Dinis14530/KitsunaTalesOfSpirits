using UnityEngine;

// Inimigo segue jogador apenas no eixo X
public class AiChace : MonoBehaviour
{
    public GameObject player; // Player
    public float speed = 3f; // Velocidade do inimigo
    public float distance; // Distância até o player

    private EnemyKnockBack knockBack;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        knockBack = GetComponent<EnemyKnockBack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null)
            return;

        // Mantem a distancia atualizada para decidir quando perseguir
        distance = Vector2.Distance(transform.position, player.transform.position);

        // Usa a posicao do player para virar o inimigo na direcao correta
        bool playerIsLeft = player.transform.position.x < transform.position.x;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = playerIsLeft;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (playerIsLeft ? -1f : 1f);
            transform.localScale = scale;
        }

        // Persegue apenas no eixo X e bloqueia movimento durante o knockback
        if (distance < 20 && (knockBack == null || !knockBack.isKnockback))
        {
            Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
        }
    }
}
