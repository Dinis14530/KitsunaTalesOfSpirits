using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    [Header("Menu Options")]
    [SerializeField] private GameObject optionsMenu; 
    [SerializeField] private GameObject mainMenu;  

    // Este método é chamado pelo botão OnClick()
    public void OpenOptions()
    {
        if (optionsMenu != null)
            optionsMenu.SetActive(true); // Ativa o menu de opções
        
        if (mainMenu != null)
            mainMenu.SetActive(false);   // Desativa o main menu
    }

    public void CloseOptions()
    {
        if (optionsMenu != null)
            optionsMenu.SetActive(false); // Desativa o menu de opções
        
        if (mainMenu != null)
            mainMenu.SetActive(true);    // Reativa o main menu
    }
}