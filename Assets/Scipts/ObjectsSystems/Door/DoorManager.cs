using UnityEngine;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour
{
    public static DoorManager Instance { get; private set; }
    
    private HashSet<string> openedDoors = new HashSet<string>();

    private void Awake()
    {
        // Se já existe uma instância, destrói este
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("DoorManager inicializado");
    }

    public void MarkDoorAsOpened(string doorID)
    {
        if (string.IsNullOrEmpty(doorID)) return;
        
        if (!openedDoors.Contains(doorID))
        {
            openedDoors.Add(doorID);
            Debug.Log($"Porta marcada como aberta: {doorID}");
        }
    }

    public bool IsDoorOpened(string doorID)
    {
        if (string.IsNullOrEmpty(doorID)) return false;
        return openedDoors.Contains(doorID);
    }

    public List<string> GetOpenedDoors()
    {
        return new List<string>(openedDoors);
    }

    public void SetOpenedDoors(List<string> doorIDs)
    {
        openedDoors.Clear();
        if (doorIDs != null)
        {
            foreach (var id in doorIDs)
            {
                if (!string.IsNullOrEmpty(id))
                    openedDoors.Add(id);
            }
        }
        Debug.Log($"Portas carregadas: {openedDoors.Count}");
    }

    public void ClearAllDoors()
    {
        openedDoors.Clear();
    }
}