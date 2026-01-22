using UnityEngine;

public class AttackPosFollower : MonoBehaviour
{
    public PlayerController playerController; // Referência ao PlayerController
    public float offsetX = 20f; // Distância horizontal à frente do player
    public float offsetY = 0f; // Distância vertical

    void LateUpdate()
    {
        // Usa o flipX do sprite para definir a direção
        float direction = playerController.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;

        transform.localPosition = new Vector3(direction * Mathf.Abs(offsetX), offsetY, 0f);
    }
}
