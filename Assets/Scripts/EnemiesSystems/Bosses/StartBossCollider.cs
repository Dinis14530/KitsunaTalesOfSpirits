using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StartBossCollider : MonoBehaviour
{
    [SerializeField] private FoxBoss foxBoss;

    private Collider2D startCollider;
    private bool bossStarted;

    void Awake()
    {
        startCollider = GetComponent<Collider2D>();
        startCollider.isTrigger = true;

        if (foxBoss == null)
            foxBoss = FindObjectOfType<FoxBoss>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (bossStarted || !col.CompareTag("Player") || foxBoss == null)
            return;

        bossStarted = true;
        foxBoss.StartBossFight();
        startCollider.enabled = false;
    }
}
