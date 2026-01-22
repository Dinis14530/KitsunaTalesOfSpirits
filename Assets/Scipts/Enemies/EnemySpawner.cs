using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    [Header("Zona de Spawn")]
    public BoxCollider2D spawnArea;
    public float spawnDistance = 12f;
    public float despawnDistance = 18f;
    public Tilemap trailTilemap;
    public Tilemap requiredSourceTilemap;
    GameObject enemy;
    Transform player;
    bool waitingForReenter;
    bool hadEnemyLastFrame;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (!player || !spawnArea) return;

        float dist = Vector2.Distance(player.position, spawnArea.bounds.center);

        if (hadEnemyLastFrame && enemy == null)
            waitingForReenter = true;

        hadEnemyLastFrame = enemy != null;

        if (dist > despawnDistance)
            waitingForReenter = false;

        if (!enemy && !waitingForReenter && dist >= spawnDistance)
            Spawn();
            Debug.Log("Spawned Enemy");

        if (enemy && dist >= despawnDistance)
            Despawn();
            Debug.Log("Despawned Enemy");
    }

    void Spawn()
    {
        Vector2 pos = GetRandomPointInBounds(spawnArea.bounds);

        enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        var ai = enemy.GetComponentInChildren<AiChace>();
        if (ai) ai.player = player.gameObject;

        var trail = enemy.GetComponentInChildren<EnemyTrailTilemap>();
        if (trail)
        {
            trail.trailTilemap = trailTilemap;
            trail.requiredSourceTilemap = requiredSourceTilemap
                ? requiredSourceTilemap
                : GameObject.Find("Tilemap")?.GetComponent<Tilemap>();
        }
    }

    void Despawn()
    {
        enemy.GetComponentInChildren<EnemyTrailTilemap>()?.ClearAllTrail();
        Destroy(enemy);
        enemy = null;
    }

    Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }
}
