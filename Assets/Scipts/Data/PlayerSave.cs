using System;
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

    private IEnumerator Start()
    {
        CacheReferences();

        Task<LoadedSave> loadTask = LoadBestSaveAsync();

        while (!loadTask.IsCompleted)
            yield return null;

        if (loadTask.Exception != null)
        {
            Debug.LogException(loadTask.Exception);
            yield break;
        }

        LoadedSave loadedSave = loadTask.Result;

        if (loadedSave == null || loadedSave.Data == null)
        {
            SaveGame();
            Debug.Log("Primeiro save criado");

            yield break;
        }

        ApplySaveData(loadedSave.Data);

        if (loadedSave.FromCloud)
        {
            SaveSystem.Save(loadedSave.Data);
        }
        else
        {
            _ = CloudSaveSync.SaveAsync(loadedSave.Data);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Jogo salvo ao sair");
    }

    public void SaveGame()
    {
        SaveData data = CreateSaveData();
        SaveSystem.Save(data);
        _ = CloudSaveSync.SaveAsync(data);
    }

    public void SaveGameAsync()
    {
        StartCoroutine(SaveGameCoroutine());
    }

    public IEnumerator SaveGameCoroutine()
    {
        SaveData data = CreateSaveData();
        SaveSystem.Save(data);
        Task saveTask = SaveSystem.SaveAsync(data);
        Task cloudTask = CloudSaveSync.SaveAsync(data);

        while (!saveTask.IsCompleted || !cloudTask.IsCompleted)
            yield return null;

        if (saveTask.Exception != null)
            Debug.LogException(saveTask.Exception);

        if (cloudTask.Exception != null)
            Debug.LogException(cloudTask.Exception);

        if (saveTask.Exception == null && cloudTask.Exception == null)
            Debug.Log("Jogo guardado em segundo plano");
    }

    public void LoadGame()
    {
        ApplySaveData(SaveSystem.Load());
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
        data.lastSavedUtcTicks = DateTime.UtcNow.Ticks;

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

    private IEnumerator LoadBestSaveCoroutine()
    {
        Task<LoadedSave> loadTask = LoadBestSaveAsync();

        while (!loadTask.IsCompleted)
            yield return null;

        if (loadTask.Exception != null)
        {
            Debug.LogException(loadTask.Exception);
            yield break;
        }

        LoadedSave loadedSave = loadTask.Result;

        if (loadedSave == null || loadedSave.Data == null)
        {
            SaveGame();
            yield break;
        }

        ApplySaveData(loadedSave.Data);

        if (loadedSave.FromCloud)
            SaveSystem.Save(loadedSave.Data);
        else
            _ = CloudSaveSync.SaveAsync(loadedSave.Data);
    }

    private async Task<LoadedSave> LoadBestSaveAsync()
    {
        SaveData localData = SaveSystem.HasSave() ? SaveSystem.Load() : null;
        SaveData cloudData = await CloudSaveSync.LoadAsync();

        if (localData == null && cloudData == null)
            return null;

        if (localData == null)
            return new LoadedSave { Data = cloudData, FromCloud = true };

        if (cloudData == null)
            return new LoadedSave { Data = localData, FromCloud = false };

        if (cloudData.lastSavedUtcTicks > localData.lastSavedUtcTicks)
            return new LoadedSave { Data = cloudData, FromCloud = true };

        return new LoadedSave { Data = localData, FromCloud = false };
    }

    private void ApplySaveData(SaveData data)
    {
        CacheReferences();

        if (data == null)
            return;

        transform.position = data.playerPosition;
        health = data.playerHealth;
        currentCheckpoint = data.activeCheckpoint;

        if (playerHealth != null)
        {
            playerHealth.maxHealth = data.playerMaxHealth;
            playerHealth.RestoreHealth(data.playerHealth);
        }

        if (inventoryManager != null && data.inventoryItems != null)
            inventoryManager.ImportInventory(data.inventoryItems);

        if (coinDisplay != null)
            coinDisplay.SetCoins(data.coins);

        if (MapManager.Instance != null)
            MapManager.Instance.SetMapPurchased(data.hasPurchasedMap);

        if (playerDash != null)
            playerDash.canDash = data.canDash;

        if (ChestManager.Instance != null && data.openedChests != null)
            ChestManager.Instance.SetOpenedChests(data.openedChests);

        if (DoorManager.Instance != null && data.openedDoors != null)
            DoorManager.Instance.SetOpenedDoors(data.openedDoors);

        if (BossManager.Instance != null && data.defeatedBosses != null)
            BossManager.Instance.SetDefeatedBosses(data.defeatedBosses);
    }

    private class LoadedSave
    {
        public SaveData Data;
        public bool FromCloud;
    }
}
