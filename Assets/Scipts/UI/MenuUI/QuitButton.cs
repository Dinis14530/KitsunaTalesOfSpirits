using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {

        // Fecha o jogo no build
        Application.Quit();

        // Fecha o jogo no editor 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
