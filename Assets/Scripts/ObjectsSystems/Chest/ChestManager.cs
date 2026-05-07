using UnityEngine;
using System.Collections.Generic;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance { get; private set; }
    
    private HashSet<string> openedChests = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkChestAsOpened(string chestID)
    {
        if (!openedChests.Contains(chestID))
            openedChests.Add(chestID);
    }

    public bool IsChestOpened(string chestID)
    {
        return openedChests.Contains(chestID);
    }

    public List<string> GetOpenedChests()
    {
        return new List<string>(openedChests);
    }

    public void SetOpenedChests(List<string> chestIDs)
    {
        openedChests.Clear();
        if (chestIDs != null)
        {
            foreach (var id in chestIDs)
            {
                openedChests.Add(id);
            }
        }
    }

    public void ClearAllChests()
    {
        openedChests.Clear();
    }
}