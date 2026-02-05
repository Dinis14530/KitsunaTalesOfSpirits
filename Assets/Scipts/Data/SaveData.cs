using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int playerHealth;
    public int playerMaxHealth;  
    public string activeCheckpoint;
    public List<InventoryItemData> inventoryItems = new List<InventoryItemData>();
    public int coins;
    public List<string> unlockedAbilities = new List<string>();
    
    // Habilidades específicas do jogador
    public bool canDash = false;
    
    // Baús, Portas e Bosses
    public List<string> openedChests = new List<string>();
    public List<string> openedDoors = new List<string>();
    public List<string> defeatedBosses = new List<string>();
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int quantity;
    public string itemDescription;
}