using UnityEngine;

public class LampFollower : MonoBehaviour
{
    public EnemySwim enemySwim; // Referência ao EnemySwim
    public float offsetX = 10f; // Distância horizontal à frente do inimigo
    public float offsetY = 5f; // Distância vertical

    void LateUpdate()
    {
        if (enemySwim == null) return;

        // Usa o flipX do sprite do EnemySwim para definir a direção
        float direction = enemySwim.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;

        transform.localPosition = new Vector3(direction * Mathf.Abs(offsetX), offsetY, 0f);
    }
}