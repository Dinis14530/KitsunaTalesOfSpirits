using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject menuCanvas;

    void Start()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(true);
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current == null)
            return;

        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TrySaveBeforeReturn();
            SceneManager.LoadScene(0);
        }
    }

    public void CloseMenu()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
    }

    private void TrySaveBeforeReturn()
    {
        var playerSave = FindFirstObjectByType<PlayerSave>();
        if (playerSave != null)
            playerSave.SaveGame();
    }
}
