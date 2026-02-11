using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    public int health = 5;
    public string currentCheckpoint = "Campfire_01";

    void Start()
    {
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
        SaveData data = new SaveData();
        data.playerPosition = transform.position;
        
        // Salva vida actual e máxima
        var playerHealth = GetComponent<PlayerHealth>();
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

        // Salva inventário
        var inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();
        if (inventoryManager != null)
            data.inventoryItems = inventoryManager.ExportInventory();

        // Salva moedas
        var coinDisplay = FindFirstObjectByType<CoinDisplay>();
        if (coinDisplay != null)
            data.coins = coinDisplay.GetCoins();

        // Salva habilidades desbloqueadas
        var playerDash = GetComponent<PlayerDash>();
        if (playerDash != null)
            data.canDash = playerDash.canDash;

        // Salva baús abertos
        if (ChestManager.Instance != null)
            data.openedChests = ChestManager.Instance.GetOpenedChests();

        // Salva portas abertas
        if (DoorManager.Instance != null)
            data.openedDoors = DoorManager.Instance.GetOpenedDoors();

        // Salva bosses derrotados
        if (BossManager.Instance != null)
            data.defeatedBosses = BossManager.Instance.GetDefeatedBosses();

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        transform.position = data.playerPosition;
        health = data.playerHealth;
        currentCheckpoint = data.activeCheckpoint;

        // Carrega vida máxima e atual
        var playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.maxHealth = data.playerMaxHealth;
            playerHealth.RestoreHealth(data.playerHealth);
        }

        // Carrega inventário
        var inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();
        if (inventoryManager != null && data.inventoryItems != null)
            inventoryManager.ImportInventory(data.inventoryItems);

        // Carrega moedas
        var coinDisplay = FindFirstObjectByType<CoinDisplay>();
        if (coinDisplay != null)
            coinDisplay.SetCoins(data.coins);

        // Carrega habilidades desbloqueadas
        var playerDash = GetComponent<PlayerDash>();
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
}
