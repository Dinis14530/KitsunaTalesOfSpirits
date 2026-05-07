using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        // Carrega estado guardado (ou true se não existir)
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        // Define fullscreen mas sem disparar o listener
        toggle.SetIsOnWithoutNotify(isFullscreen);
        Screen.fullScreen = isFullscreen;

        // Liga o listener
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void OnToggleChanged(bool isFullscreen)
    {
        // Muda fullscreen
        Screen.fullScreen = isFullscreen;

        // Guarda a preferência
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}