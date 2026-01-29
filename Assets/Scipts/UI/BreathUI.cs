using UnityEngine;
using UnityEngine.UI;

public class BreathUI : MonoBehaviour
{
    [Header("Referências")]
    public PlayerSwim playerSwim;   // referência ao PlayerSwim
    public Image breathImage;       // Image do Canvas para o fôlego
    public Sprite[] breathSprites;  // sprites da barra de fôlego 

    private int lastSpriteIndex = -1;

    void Start()
    {
        UpdateBreathBar();
    }

    void Update()
    {
        if (playerSwim != null && playerSwim.isInWater)
        {
            breathImage.enabled = true;
            UpdateBreathBar();
        }
        else
        {
            breathImage.enabled = false;
        }
    }

void UpdateBreathBar()
    {
        if (playerSwim == null || breathSprites.Length == 0) return;

        float breathPercent = playerSwim.currentBreathTime / playerSwim.maxBreathTime;

        int index = Mathf.FloorToInt((1f - breathPercent) * breathSprites.Length);
        index = Mathf.Clamp(index, 0, breathSprites.Length - 1);

        if (index != lastSpriteIndex)
        {
            breathImage.sprite = breathSprites[index];
            lastSpriteIndex = index;
        }

        if (index == breathSprites.Length - 1)
        {
            playerSwim.currentBreathTime = 0f;
        }
    }

}