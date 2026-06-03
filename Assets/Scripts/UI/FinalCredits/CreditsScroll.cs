using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    public float speed = 50f;

    [SerializeField]
    private int menuSceneBuildIndex = 0;

    [SerializeField]
    private RectTransform endPoint;

    private RectTransform rectTransform;
    private bool finished;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (finished)
            return;

        rectTransform.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (rectTransform.position.y >= endPoint.position.y)
        {
            finished = true;
            SceneManager.LoadScene(menuSceneBuildIndex);
        }
    }
}
