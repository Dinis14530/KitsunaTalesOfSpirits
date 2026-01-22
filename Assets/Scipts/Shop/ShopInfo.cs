using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
        if (infoPanel != null)
        {
            infoPanel.alpha = 0;
            infoPanel.blocksRaycasts = false;
            infoPanel.interactable = false;
            infoPanel.gameObject.SetActive(true);
        }
    }

    public void ShowItemInfo(ItemSO itemSO)
    {
        if (infoPanel == null || itemSO == null) return;

        itemNameText.text = itemSO.itemName;
        itemDescriptionText.text = itemSO.itemDescription;

        infoPanel.alpha = 1;
        infoPanel.blocksRaycasts = true;
        infoPanel.interactable = true;
    }

    public void HideItemInfo()
    {
        if (infoPanel == null) return;

        infoPanel.alpha = 0;
        infoPanel.blocksRaycasts = false;
        infoPanel.interactable = false;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void FollowMouse()
    {
        if (infoPanelRect == null || infoPanel == null) return;
        if (infoPanel.alpha <= 0) return; 

        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);
        infoPanelRect.position = mousePosition + offset;
    }
}