using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Liga os métodos públicos ao OnClick de um botão no menu para apagar o save.
public class SaveResetter : MonoBehaviour
{
    // Flag estática: quando true, PlayerSave ignora qualquer save existente no próximo load.
    public static bool JustReset { get; private set; }

    public static void ConsumeReset()
    {
        JustReset = false;
    }

    [Header("Novo Jogo")]
    public int gameplaySceneIndex = 1;

    private bool isResetting;

    // Apaga o save (local + cloud) e reinicia os estados, ficando no ecrã atual.
    public void DeleteSave()
    {
        if (isResetting)
            return;

        ClearRuntimeProgress();
        OverwriteWithFreshSave();
        JustReset = true;

        Debug.Log("Save apagado");
    }

    // Apaga o save (local + cloud) e só depois arranca um jogo novo.
    public async void DeleteSaveAndStartNewGame()
    {
        if (isResetting)
            return;

        isResetting = true;

        ClearRuntimeProgress();

        SaveData fresh = CreateFreshSave();
        SaveSystem.Save(fresh);
        await CloudSaveSync.SaveAsync(fresh);

        JustReset = true;

        Debug.Log("Save apagado, a iniciar jogo novo");
        SceneManager.LoadScene(gameplaySceneIndex);
    }

    private void OverwriteWithFreshSave()
    {
        SaveData fresh = CreateFreshSave();
        SaveSystem.Save(fresh);
        _ = CloudSaveSync.SaveAsync(fresh);
    }

    private SaveData CreateFreshSave()
    {
        return new SaveData { lastSavedUtcTicks = DateTime.UtcNow.Ticks };
    }

    private void ClearRuntimeProgress()
    {
        if (ChestManager.Instance != null)
            ChestManager.Instance.ClearAllChests();

        if (DoorManager.Instance != null)
            DoorManager.Instance.ClearAllDoors();

        if (BossManager.Instance != null)
            BossManager.Instance.ClearAllBosses();
    }
}
