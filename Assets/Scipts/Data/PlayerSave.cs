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

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerPosition = transform.position;
        data.playerHealth = health;
        data.activeCheckpoint = currentCheckpoint;

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();
        if (data == null) return;

        transform.position = data.playerPosition;
        health = data.playerHealth;
        currentCheckpoint = data.activeCheckpoint;
    }
}
