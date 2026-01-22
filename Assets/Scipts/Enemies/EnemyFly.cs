using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public float speed = 3f;
    public float stopDistance = 2f;
    public float verticalOffset = 1.5f;
    public bool isChasing = false;
    private GameObject player;
    private EnemyKnockBack knockback;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        knockback = GetComponent<EnemyKnockBack>();
    }

    void Update()
    {
        if (player == null) return;
        if (knockback != null && knockback.isKnockback) return;

        if (isChasing)
            Chase();

        Flip();
    }

    private void Chase()
    {
        Vector2 targetPosition = new Vector2(
            player.transform.position.x,
            player.transform.position.y + verticalOffset
        );

        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance > stopDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
        }
    }

    private void Flip()
    {
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
