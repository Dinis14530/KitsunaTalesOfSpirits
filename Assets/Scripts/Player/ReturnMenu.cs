using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMenu : MonoBehaviour
{
    [System.Obsolete]
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Salva o jogo antes de voltar ao menu
            var playerSave = FindObjectOfType<PlayerSave>();
            if (playerSave != null)
                playerSave.SaveGame();

            SceneManager.LoadScene(0); 
            Debug.Log(Application.persistentDataPath);
        }
    }
}
