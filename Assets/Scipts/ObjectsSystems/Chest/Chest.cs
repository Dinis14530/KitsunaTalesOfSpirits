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

    [Header("Áudio")]
    public AudioSource audioSource;   // Fonte de som do baú
    public AudioClip openClip;        // Som ao abrir

    void Start()
    {
        ChestID = GlobalHelper.GenerateUniqueID(gameObject);

        // Se não tiver AudioSource, tenta pegar
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Verifica se este baú já foi aberto antes
        if (ChestManager.Instance != null && ChestManager.Instance.IsChestOpened(ChestID))
        {
            SetOpened(true);
        }
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

        // Toca o som ao abrir
        PlayOpenSound();

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

    private void PlayOpenSound()
    {
        if (audioSource != null && openClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // pitch aleatório
            audioSource.PlayOneShot(openClip);
        }
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
            // Guarda que este baú foi aberto
            if (ChestManager.Instance != null)
                ChestManager.Instance.MarkChestAsOpened(ChestID);
        }
    }
}
