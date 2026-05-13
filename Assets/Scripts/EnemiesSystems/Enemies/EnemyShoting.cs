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

        if (projectilePrefab == null || bulletPosition == null)
            return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < 10)
        {
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
        ObjectPoolManager.Spawn(projectilePrefab, bulletPosition.position, Quaternion.identity);
    }

    private void TryFindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
