using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }

    private HashSet<string> defeatedBosses = new HashSet<string>();
    private Dictionary<string, int> bossHealths = new Dictionary<string, int>();

    private void Awake()
    {
        // Se já existe uma instância, destrói este
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void MarkBossAsDefeated(string bossID)
    {
        if (string.IsNullOrEmpty(bossID))
            return;

        if (!defeatedBosses.Contains(bossID))
        {
            defeatedBosses.Add(bossID);
            Debug.Log($"Boss marcado como derrotado: {bossID}");
        }
    }

    public bool IsBossDefeated(string bossID)
    {
        if (string.IsNullOrEmpty(bossID))
            return false;
        return defeatedBosses.Contains(bossID);
    }

    public List<string> GetDefeatedBosses()
    {
        return new List<string>(defeatedBosses);
    }

    public void SetDefeatedBosses(List<string> bossIDs)
    {
        defeatedBosses.Clear();
        if (bossIDs != null)
        {
            foreach (var id in bossIDs)
            {
                if (!string.IsNullOrEmpty(id))
                    defeatedBosses.Add(id);
            }
        }
        Debug.Log($"Bosses carregados como derrotados: {defeatedBosses.Count}");
    }

    public void ClearAllBosses()
    {
        defeatedBosses.Clear();
        bossHealths.Clear();
    }

    public void SetBossHealth(string bossID, int health)
    {
        if (!string.IsNullOrEmpty(bossID))
            bossHealths[bossID] = health;
    }

    public int GetBossHealth(string bossID)
    {
        if (!string.IsNullOrEmpty(bossID) && bossHealths.TryGetValue(bossID, out int hp))
            return hp;
        return -1;
    }

    public List<BossHealthEntry> GetBossHealths()
    {
        var list = new List<BossHealthEntry>();
        foreach (var kvp in bossHealths)
        {
            list.Add(new BossHealthEntry { bossID = kvp.Key, health = kvp.Value });
        }
        return list;
    }

    public void SetBossHealths(List<BossHealthEntry> entries)
    {
        bossHealths.Clear();
        if (entries == null)
            return;
        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.bossID))
                bossHealths[entry.bossID] = entry.health;
        }
    }
}
