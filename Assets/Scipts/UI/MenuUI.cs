using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas; // arrasta o MenuCanvas aqui no Inspector

    void Awake()
    {
        if (menuCanvas == null)
        {
            Debug.LogError("[MenuUI] MenuCanvas não atribuído!");
            return;
        }

        // Liga o menu ao iniciar
        menuCanvas.SetActive(true);
        Debug.Log("[MenuUI] MenuCanvas ativado no início do jogo");
    }

    // Função para fechar o menu 
    public void CloseMenu()
    {
        menuCanvas.SetActive(false);
        Debug.Log("[MenuUI] MenuCanvas fechado");
    }
}
