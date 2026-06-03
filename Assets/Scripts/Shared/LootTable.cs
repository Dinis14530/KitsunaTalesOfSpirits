using UnityEngine;

[System.Serializable]
public class LootDrop
{
    public ItemSO itemSO;

    [Range(0, 100)]
    public int dropChance = 100;
    public int quantity = 1;
}

public static class LootTable
{
    public static void TryDrop(LootDrop[] entries, Vector3 position)
    {
        if (entries == null || entries.Length == 0)
            return;

        foreach (LootDrop entry in entries)
        {
            if (entry.itemSO == null)
                continue;

            int roll = Random.Range(0, 101);
            if (roll <= entry.dropChance)
            {
                LootHelper.SpawnLootItem(entry.itemSO, position, entry.quantity);
                Debug.Log($"{entry.itemSO.itemName} dropped");
            }
        }
    }
}
