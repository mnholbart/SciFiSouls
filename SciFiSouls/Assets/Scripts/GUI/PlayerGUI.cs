using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

    Player player {
        get { return PlayerManager.instance.Player; }
    }
    
    //Health bar
    public Image HealthBarImage;
    public Color FullHealthColor = Color.green;
    public Color EmptyHealthColor = Color.red;
    //Stamina Bar
    public Image StaminaBarImage;
    public Color FullStaminaColor = Color.blue;
    public Color EmptyStaminaColor = Color.red;

    void Update() {
        if (!player)
            return;

        DrawHealthBar();
        DrawStaminaBar();
    }

    void DrawHealthBar() {
        if (!player.health.isDirty)
            return;
        player.health.isDirty = false;
        float currentHealth = player.health.CurrentHealth;
        float maxHealth = player.health.MaxHealth;
        currentHealth = Mathf.Max(currentHealth, 0);
        maxHealth = Mathf.Max(maxHealth, 1);
        float percent = (float)currentHealth / (float)maxHealth;
        percent = Mathf.Clamp(percent, 0, 1.0f);
        HealthBarImage.transform.localScale = new Vector3(percent, 1, 1);
        HealthBarImage.color = Color.Lerp(EmptyHealthColor, FullHealthColor, percent);
    }

    void DrawStaminaBar() {
        if (!player.stamina.isDirty)
            return;
        player.stamina.isDirty = false;
        float currentStamina = player.stamina.CurrentStamina;
        float maxStamina = player.stamina.MaximumStamina;
        currentStamina = Mathf.Max(currentStamina, 0);
        maxStamina = Mathf.Max(maxStamina, 1);
        float percent = currentStamina / maxStamina;
        percent = Mathf.Clamp(percent, 0, 1.0f);
        StaminaBarImage.transform.localScale = new Vector3(percent, 1, 1);
        StaminaBarImage.color = Color.Lerp(EmptyStaminaColor, FullStaminaColor, percent);
    }
}
