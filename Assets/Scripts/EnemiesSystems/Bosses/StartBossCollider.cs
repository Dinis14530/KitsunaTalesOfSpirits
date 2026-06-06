using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StartBossCollider : MonoBehaviour
{
    [SerializeField] private PaleWarriorBoss paleWarriorBoss;

    private Collider2D startCollider;
    private bool bossStarted;

    void Awake()
    {
        startCollider = GetComponent<Collider2D>();
        startCollider.isTrigger = true;

        if (paleWarriorBoss == null)
            paleWarriorBoss = FindObjectOfType<PaleWarriorBoss>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (bossStarted || !col.CompareTag("Player") || paleWarriorBoss == null)
            return;

        bossStarted = true;
        paleWarriorBoss.StartBossFight();
        startCollider.enabled = false;
    }
}
