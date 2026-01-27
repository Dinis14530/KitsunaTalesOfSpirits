using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas; // arrasta o MenuCanvas aqui no Inspector

    void Awake()
    {
        // Liga o menu ao iniciar
        menuCanvas.SetActive(true);
    }

    // Função para fechar o menu 
    public void CloseMenu()
    {
        menuCanvas.SetActive(false);
    }
}
