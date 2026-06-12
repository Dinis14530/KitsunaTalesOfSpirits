using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMenu : MonoBehaviour
{
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current == null)
            return;

        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            try
            {
                var playerSave = FindObjectOfType<PlayerSave>();
                if (playerSave != null)
                    playerSave.SaveGame();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            SceneManager.LoadScene(0);
            Debug.Log(Application.persistentDataPath);
        }
    }
}
