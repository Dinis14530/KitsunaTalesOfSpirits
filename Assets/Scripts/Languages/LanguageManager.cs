using UnityEngine;

public enum Language
{
    English = 0,
    Portuguese = 1,
    Spanish = 2,
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public Language currentLanguage = Language.Portuguese;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Carrega idioma guardado
            if (PlayerPrefs.HasKey("GameLanguage"))
            {
                currentLanguage = (Language)PlayerPrefs.GetInt("GameLanguage");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(Language newLanguage)
    {
        currentLanguage = newLanguage;
    }
}