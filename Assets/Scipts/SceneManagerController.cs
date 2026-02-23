using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KeepOnlyOneScene : MonoBehaviour
{
    [SerializeField] private string sceneToKeep; // Nome da cena que queres manter

    private void Start()
    {
        StartCoroutine(KeepOnlyScene());
    }

    private IEnumerator KeepOnlyScene()
    {
        // Espera 1 frame para garantir que todas as cenas estão carregadas
        yield return null;

        Scene activeScene = SceneManager.GetSceneByName(sceneToKeep);

        if (!activeScene.IsValid())
        {
            Debug.LogError("Cena não encontrada: " + sceneToKeep);
            yield break;
        }

        // Define como ativa
        SceneManager.SetActiveScene(activeScene);

        // Percorre todas as cenas abertas
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name != sceneToKeep)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}