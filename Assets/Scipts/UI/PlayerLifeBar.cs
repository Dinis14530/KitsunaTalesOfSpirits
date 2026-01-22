using UnityEngine;
using UnityEngine.UI;

// Player LifeBar
public class HealthDisplay : MonoBehaviour
{
    // Sprites
    public Image healthImg;
    public Sprite healthFullLife;
    public Sprite health_4;
    public Sprite health_3;
    public Sprite health_2;
    public Sprite health_1;

    public void UpdateHealth(float health) // Funcao que muda a UI dependo da vida
    {
        // Se a vida for x entao sprite é y
        if (health == 5)
            healthImg.sprite = healthFullLife;
        else if (health == 4)
            healthImg.sprite = health_4;
        else if (health == 3)
            healthImg.sprite = health_3;
        else if (health == 2)
            healthImg.sprite = health_2;
        else if (health == 1)
            healthImg.sprite = health_1;
    }
}
