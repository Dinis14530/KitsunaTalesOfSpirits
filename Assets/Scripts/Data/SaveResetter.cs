using UnityEngine;
using UnityEngine.SceneManagement;

// Liga os métodos públicos ao OnClick de um botão no menu para apagar o save.
public class SaveResetter : MonoBehaviour
{
    [Header("Novo Jogo")]
    public int gameplaySceneIndex = 1;

    private bool isResetting;

    // Apaga o save (local + cloud) e reinicia os estados, ficando no ecrã atual.
    public void DeleteSave()
    {
        if (isResetting)
            return;

        SaveSystem.DeleteSave();
        _ = CloudSaveSync.DeleteAsync();
        ClearRuntimeProgress();

        Debug.Log("Save apagado");
    }

    // Apaga o save (local + cloud) e só depois arranca um jogo novo.
    public async void DeleteSaveAndStartNewGame()
    {
        if (isResetting)
            return;

        isResetting = true;

        SaveSystem.DeleteSave();
        await CloudSaveSync.DeleteAsync();
        ClearRuntimeProgress();

        Debug.Log("Save apagado, a iniciar jogo novo");
        SceneManager.LoadScene(gameplaySceneIndex);
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
