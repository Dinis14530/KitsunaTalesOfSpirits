// using UnityEngine;
// using UnityEngine.UI;

// public class SaveUI : MonoBehaviour
// {
//     [SerializeField] private Button[] saveButtons; // Botões para salvar em slots
//     [SerializeField] private Button[] loadButtons; // Botões para carregar saves
//     [SerializeField] private Text[] saveTexts; // Textos indicando se save existe

//     private void Start()
//     {
//         for (int i = 0; i < saveButtons.Length; i++)
//         {
//             int slot = i + 1;
//             saveButtons[i].onClick.AddListener(() => OnSaveButtonClicked(slot));

//             if (loadButtons.Length > i)
//             {
//                 loadButtons[i].onClick.AddListener(() => OnLoadButtonClicked(slot));
//             }

//             UpdateSaveText(slot);
//         }
//     }

//     private void OnSaveButtonClicked(int slot)
//     {
//         // Implementar save aqui quando pronto
//         Debug.Log("Salvando no slot " + slot);
//         // SaveManager.Instance.SaveCurrentGame(slot);
//         UpdateSaveText(slot);
//     }

//     // private void OnLoadButtonClicked(int slot)
//     // {
//     //     // Implementar load aqui quando pronto
//     //     Debug.Log("Carregando do slot " + slot);
//     //     // SaveManager.Instance.LoadCurrentGame(slot);
//     // }

//     private void UpdateSaveText(int slot)
//     {
//         if (saveTexts.Length >= slot)
//         {
//             // Simular se save existe (implementar com SaveManager depois)
//             bool exists = false; // SaveManager.Instance.SaveExists(slot);
//             saveTexts[slot - 1].text = exists ? "Slot " + slot + " (Salvo)" : "Slot " + slot + " (Vazio)";
//         }
//     }
// }
