using UnityEngine;

public class Chest : MonoBehaviour, IInterectable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }

    [System.Serializable]
    public class ChestLoot
    {
        public ItemSO itemSO;
        [Range(0, 100)]
        public int dropChance = 100;
        public int quantity = 1;
    }

    public ChestLoot[] lootItems;
    public Sprite openedSprite;

    void Start()
    {
        ChestID = GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        OpenChest();
    }

    private void OpenChest()
    {
        SetOpened(true);

        if (lootItems == null || lootItems.Length == 0)
            return;

        foreach (ChestLoot loot in lootItems)
        {
            if (loot.itemSO == null) continue;

            int chance = Random.Range(0, 101);

            if (chance <= loot.dropChance)
            {
                LootHelper.SpawnLootItem(loot.itemSO, transform.position, loot.quantity);
            }
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
            GetComponent<SpriteRenderer>().sprite = openedSprite;
    }
}
