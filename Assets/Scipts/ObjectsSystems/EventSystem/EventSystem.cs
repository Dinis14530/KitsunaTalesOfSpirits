using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemCleaner : MonoBehaviour
{
    void Awake()
    {
        // Procura todos os EventSystems ativos na cena
        var systems = FindObjectsOfType<EventSystem>();

        // Se houver mais do que 1, destrói este
        if (systems.Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
