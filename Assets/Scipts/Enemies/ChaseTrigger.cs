using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    private EnemyFly enemy;

    void Start()
    {
        enemy = GetComponentInParent<EnemyFly>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.isChasing = false;
        }
    }
}
