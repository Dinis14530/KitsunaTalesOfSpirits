using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    public int health = 5;
    public string currentCheckpoint = "Campfire_01";

    private PlayerHealth playerHealth;
    private PlayerDash playerDash;
    private InventoryManager inventoryManager;
    private CoinDisplay coinDisplay;

    void Start()
    {
        CacheReferences();

        // cria save apenas se não existir
        if (!SaveSystem.HasSave())
        {
            SaveGame();
            Debug.Log("Primeiro save criado");
        }
        else
        {
            LoadGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Jogo salvo ao sair");
    }

    public void SaveGame()
    {
        SaveSystem.Save(CreateSaveData());
    }

    public void SaveGameAsync()
    {
        StartCoroutine(SaveGameCoroutine());
    }

    public IEnumerator SaveGameCoroutine()
    {
        SaveData data = CreateSaveData();
        Task saveTask = SaveSystem.SaveAsync(data);

        while (!saveTask.IsCompleted)
            yield return null;

        if (saveTask.Exception != null)
            Debug.LogException(saveTask.Exception);
        else
            Debug.Log("Jogo guardado em segundo plano");
    }

    public void LoadGame()
    {
        CacheReferences();

        SaveData data = SaveSystem.Load();
        if (data == null) return;

        transform.position = data.playerPosition;
        health = data.playerHealth;
        currentCheckpoint = data.activeCheckpoint;

        // Carrega vida máxima e atual
        if (playerHealth != null)
        {
            playerHealth.maxHealth = data.playerMaxHealth;
            playerHealth.RestoreHealth(data.playerHealth);
        }

        // Carrega inventário
        if (inventoryManager != null && data.inventoryItems != null)
            inventoryManager.ImportInventory(data.inventoryItems);

        // Carrega moedas
        if (coinDisplay != null)
            coinDisplay.SetCoins(data.coins);

        if (MapManager.Instance != null)
            MapManager.Instance.SetMapPurchased(data.hasPurchasedMap);

        // Carrega habilidades desbloqueadas
        if (playerDash != null)
            playerDash.canDash = data.canDash;

        // Carrega baús abertos
        if (ChestManager.Instance != null && data.openedChests != null)
            ChestManager.Instance.SetOpenedChests(data.openedChests);

        // Carrega portas abertas
        if (DoorManager.Instance != null && data.openedDoors != null)
            DoorManager.Instance.SetOpenedDoors(data.openedDoors);

        // Carrega bosses derrotados
        if (BossManager.Instance != null && data.defeatedBosses != null)
            BossManager.Instance.SetDefeatedBosses(data.defeatedBosses);
    }

    private void CacheReferences()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerDash == null)
            playerDash = GetComponent<PlayerDash>();

        if (inventoryManager == null)
            inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();

        if (coinDisplay == null)
            coinDisplay = FindFirstObjectByType<CoinDisplay>();
    }

    private SaveData CreateSaveData()
    {
        CacheReferences();

        SaveData data = new SaveData();
        data.playerPosition = transform.position;

        if (playerHealth != null)
        {
            data.playerHealth = (int)playerHealth.currentHealth;
            data.playerMaxHealth = (int)playerHealth.maxHealth;
        }
        else
        {
            data.playerHealth = health;
        }

        data.activeCheckpoint = currentCheckpoint;

        if (inventoryManager != null)
            data.inventoryItems = inventoryManager.ExportInventory();

        if (coinDisplay != null)
            data.coins = coinDisplay.GetCoins();

        if (MapManager.Instance != null)
            data.hasPurchasedMap = MapManager.Instance.IsMapPurchased();

        if (playerDash != null)
            data.canDash = playerDash.canDash;

        if (ChestManager.Instance != null)
            data.openedChests = ChestManager.Instance.GetOpenedChests();

        if (DoorManager.Instance != null)
            data.openedDoors = DoorManager.Instance.GetOpenedDoors();

        if (BossManager.Instance != null)
            data.defeatedBosses = BossManager.Instance.GetDefeatedBosses();

        return data;
    }
}
