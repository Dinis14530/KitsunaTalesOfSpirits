using UnityEngine;

public class PuzzleRewardDropper : MonoBehaviour
{
    [Header("Reward Settings")]
    [SerializeField]
    private ItemSO rewardItem; // Item a dropar

    [SerializeField]
    private Transform dropPosition; // Posição onde o item vai dropar

    [SerializeField]
    private bool useThisTransform = false; // Se true, usa a posição deste GameObject

    [SerializeField]
    private MusicPuzzleManager puzzleManager;

    private bool hasDropped = false; // Evita dropar múltiplas vezes

    private void Start()
    {
        puzzleManager = GetComponent<MusicPuzzleManager>();

        if (puzzleManager == null)
        {
            return;
        }

        // Subscreve ao evento de puzzle resolvido
        puzzleManager.onPuzzleSolved.AddListener(OnPuzzleSolved);
    }

    private void OnPuzzleSolved()
    {
        if (hasDropped)
            return;

        if (rewardItem == null)
        {
            return;
        }

        // Determina a posição de drop
        Vector3 spawnPosition;
        if (useThisTransform)
        {
            spawnPosition = transform.position;
        }
        else if (dropPosition != null)
        {
            spawnPosition = dropPosition.position;
        }
        else
        {
            spawnPosition = transform.position;
        }

        // Dropa o item
        LootHelper.SpawnLootItem(rewardItem, spawnPosition, 1);

        Debug.Log($"Item '{rewardItem.itemName}' dropado na posição {spawnPosition}!");
        hasDropped = true;
    }

    private void OnDestroy()
    {
        // Remove o listener quando o objeto é destruído
        if (puzzleManager != null)
        {
            puzzleManager.onPuzzleSolved.RemoveListener(OnPuzzleSolved);
        }
    }
}
