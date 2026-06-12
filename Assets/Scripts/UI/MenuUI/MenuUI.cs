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
            SceneManager.LoadScene(0);
    }

    public void CloseMenu()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
    }
}
