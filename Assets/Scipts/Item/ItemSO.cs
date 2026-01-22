using UnityEngine;
using TMPro;

    public enum StatToChange
    {
        none,
        health,
        mana,
        strength,
        velocity,
        dash,
        keydash,
    };
    
[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public StatToChange statToChange = StatToChange.none;
    public float amountToChangeStat; 
    private float velocityDuration = 10f;
    public Sprite sprite;
    [TextArea]
    public string itemDescription;
    public bool isCurrency = false;
    public bool isAbility = false;
    public bool isKeyItem = false;
    public int coinValue = 1;
    public int purchaseLimit = -1;

    public bool UseItem()
    {
        GameObject player = GameObject.FindWithTag("Player");

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (statToChange == StatToChange.health && playerHealth != null)
        {
            // Verifica se a vida nao esta cheia
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.currentHealth += amountToChangeStat;
                if (playerHealth.currentHealth > playerHealth.maxHealth)
                    playerHealth.currentHealth = playerHealth.maxHealth;

                playerHealth.healthDisplay.UpdateHealth(playerHealth.currentHealth);
                return true; // item usado
            }
            else
            {
                Debug.Log("Life is already full");
                return false; // não usado
            }
        }

        if (statToChange == StatToChange.dash)
        {
            PlayerDash dash = player.GetComponent<PlayerDash>();
            if (dash != null)
            {
                dash.canDash = true;
                return true;
            }
            return false;
        }

        
        if (statToChange == StatToChange.keydash)
        {

            return false;
        }
        
        if (statToChange == StatToChange.strength)
        {
            AtackPlayer attack = player.GetComponent<AtackPlayer>();
            if (attack != null)
            {
                attack.ApplyStrengthMultiplier(2f, amountToChangeStat); // duplica o dano por duration
                return true;
            }
            return false;
        }

        if (statToChange == StatToChange.velocity)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.ApplySpeedIncrease(amountToChangeStat, velocityDuration);
                return true;
            }
            return false;
        }

        if (isCurrency == true)
        {
            CoinDisplay coinDisplay = FindFirstObjectByType<CoinDisplay>();

                coinDisplay.AddCoins(coinValue);
                return true;
        }

        return false;
    }
}