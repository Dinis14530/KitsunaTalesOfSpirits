using UnityEngine;

public class Chest : MonoBehaviour, IInterectable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }

    public LootDrop[] lootItems;
    public Sprite openedSprite;

    [Header("Audio")]
    public AudioSource audioSource; // Fonte de som do bau
    public AudioClip openClip; // Som ao abrir

    void Start()
    {
        ChestID = GlobalHelper.GenerateUniqueID(gameObject);

        // Se nao tiver AudioSource, tenta pegar
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Verifica se este bau ja foi aberto antes
        if (ChestManager.Instance != null && ChestManager.Instance.IsTracked(ChestID))
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
        if (!CanInteract())
            return;
        OpenChest();
    }

    private void OpenChest()
    {
        SetOpened(true);

        // Toca o som ao abrir
        AudioHelper.PlayWithRandomPitch(audioSource, openClip);

        LootTable.TryDrop(lootItems, transform.position);
    }

    public void SetOpened(bool opened)
    {
        IsOpened = opened;
        if (IsOpened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
            // Guarda que este bau foi aberto
            if (ChestManager.Instance != null)
                ChestManager.Instance.MarkTracked(ChestID);
        }
    }
}
