using System;
using UnityEngine;

public class EnemyShoting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform bulletPosition;
    private float timer;
    private GameObject player;

    void Start()
    {
        TryFindPlayer();
    }

    void Update()
    {
        if (player == null)
        {
            TryFindPlayer();
            return;
        }

        // Sem referencia do prefab ou do ponto de disparo nao ha tiro possivel
        if (projectilePrefab == null || bulletPosition == null)
            return;

        // O inimigo so passa a atirar quando o player entra no raio definido
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < 10)
        {
            // Usa um timer simples para espaçar os disparos
            timer += Time.deltaTime;

            if (timer > 3)
            {
                timer = 0;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        // Reutiliza projeteis da pool em vez de instanciar sempre novos
        ObjectPoolManager.Spawn(projectilePrefab, bulletPosition.position, Quaternion.identity);
    }

    private void TryFindPlayer()
    {
        // Reatacha o alvo quando o player ja existe na cena
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
