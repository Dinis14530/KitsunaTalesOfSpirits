using TMPro;
using UnityEngine;

public class LanguageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        // Define valor inicial baseado no idioma atual
        dropdown.value = (int)LanguageManager.Instance.currentLanguage;
        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private void OnLanguageChanged(int index)
    {
        Language selectedLanguage = (Language)index;
        LanguageManager.Instance.SetLanguage(selectedLanguage);

        PlayerPrefs.SetInt("GameLanguage", index);
        PlayerPrefs.Save();
    }
}