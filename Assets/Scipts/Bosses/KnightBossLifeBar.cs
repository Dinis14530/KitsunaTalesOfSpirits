using UnityEngine;
using UnityEngine.UI;

public class BossLifeBar : MonoBehaviour
{
    [Header("Referências")]
    public BossHealth bossHealth;   // referência ao BossHealth
    public Image lifeBarImage;      // Image do Canvas
    public Sprite[] lifeSprites;    // sprites da barra 

    private int lastSpriteIndex = -1;

    void Start()
    {
        UpdateLifeBar();
    }

    void Update()
    {
        UpdateLifeBar();
    }

    void UpdateLifeBar()
    {
        // calcula percentual de vida
        float healthPercent = (float)bossHealth.health / bossHealth.healthMax;
        int index = Mathf.RoundToInt((1f - healthPercent) * (lifeSprites.Length - 1));

        index = Mathf.Clamp(index, 0, lifeSprites.Length - 1);

        // muda sprite apenas se necessário
        if (index != lastSpriteIndex)
        {
            lifeBarImage.sprite = lifeSprites[index];
            lastSpriteIndex = index;
        }
    }
}
