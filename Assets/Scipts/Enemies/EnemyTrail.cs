using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTrailTilemap : MonoBehaviour
{
    public Tilemap trailTilemap;
    public Tilemap requiredSourceTilemap;

    [Header("Trail Tiles")]
    public TileBase[] trailTiles;

    [Header("Trail Settings")]
    public float paintInterval = 0.2f;
    public float tileLifetime = 5f;

    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    private float timer;

    private Dictionary<Vector3Int, Coroutine> clearCoroutines = new();
    private HashSet<Vector3Int> paintedCells = new HashSet<Vector3Int>();

    private Collider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (trailTilemap == null)
            trailTilemap = FindObjectOfType<Tilemap>();

        if (requiredSourceTilemap == null)
        {
            GameObject go = GameObject.Find("Tilemap");
            if (go) requiredSourceTilemap = go.GetComponent<Tilemap>();
        }
    }

    void Update()
    {
        if (!trailTilemap || trailTiles == null || trailTiles.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer < paintInterval)
            return;
        timer = 0f;

        Vector3 pos = transform.position;

        if (col != null)
            pos.y = col.bounds.min.y;
        else if (sr != null)
            pos.y = sr.bounds.min.y;

        pos.y -= trailTilemap.layoutGrid.cellSize.y * 0.49f;
        pos.z = trailTilemap.transform.position.z;

        Vector3Int cell = trailTilemap.WorldToCell(pos);
        if (cell == lastCell)
            return;

        if (requiredSourceTilemap != null &&
            requiredSourceTilemap.GetTile(requiredSourceTilemap.WorldToCell(pos)) == null)
            return;

        lastCell = cell;

        // Escolhe um tile aleatório
        TileBase randomTile = trailTiles[Random.Range(0, trailTiles.Length)];
        trailTilemap.SetTile(cell, randomTile);
        paintedCells.Add(cell);

        if (tileLifetime > 0f)
        {
            if (clearCoroutines.TryGetValue(cell, out var c))
                StopCoroutine(c);

            clearCoroutines[cell] = StartCoroutine(ClearTile(cell));
        }
    }

    IEnumerator ClearTile(Vector3Int cell)
    {
        yield return new WaitForSeconds(tileLifetime);

        if (trailTilemap)
            trailTilemap.SetTile(cell, null);

        clearCoroutines.Remove(cell);
        paintedCells.Remove(cell);
    }

    public void ClearAllTrail()
    {
        if (trailTilemap == null)
            return;

        foreach (var kv in clearCoroutines)
            StopCoroutine(kv.Value);

        clearCoroutines.Clear();

        foreach (var cell in paintedCells)
            trailTilemap.SetTile(cell, null);

        paintedCells.Clear();
    }

    void OnDestroy()
    {
        ClearAllTrail();
    }
}
