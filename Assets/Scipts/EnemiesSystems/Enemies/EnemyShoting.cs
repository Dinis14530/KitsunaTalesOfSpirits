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
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
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
        Instantiate(projectilePrefab, bulletPosition.position, Quaternion.identity);
    }
}
