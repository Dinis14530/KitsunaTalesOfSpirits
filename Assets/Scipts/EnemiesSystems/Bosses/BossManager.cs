using UnityEngine;
using System.Collections.Generic;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }
    
    private HashSet<string> defeatedBosses = new HashSet<string>();

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
        if (string.IsNullOrEmpty(bossID)) return;
        
        if (!defeatedBosses.Contains(bossID))
        {
            defeatedBosses.Add(bossID);
            Debug.Log($"Boss marcado como derrotado: {bossID}");
        }
    }

    public bool IsBossDefeated(string bossID)
    {
        if (string.IsNullOrEmpty(bossID)) return false;
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
    }
}