using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class GUIManager : MonoBehaviour {

    public static GUIManager instance;

    public GameObject MainCanvas;

    //Player UI
    //Health bar
    public Image HealthBarImage;
    public Color FullHealthColor = Color.green;
    public Color EmptyHealthColor = Color.red;
    //Stamina Bar
    public Image StaminaBarImage;
    public Color FullStaminaColor = Color.blue;
    public Color EmptyStaminaColor = Color.red;

    void Start() {
        if (instance != null) {
            Debug.LogError("Attempted to create more than one GUIManager instance");
            Destroy(instance);
        }
        instance = this;

        MainCanvas.SetActive(true);
    }

    public void PlayerReceiveDamage(int amount, int currentHealth, int maxHealth) {
        UpdateHealthBarImage(currentHealth, maxHealth);
    }

    public void PlayerReceiveHealing(int amount, int currentHealth, int maxHealth) {
        UpdateHealthBarImage(currentHealth, maxHealth);
    }

    public void UpdatePlayerHealthbar(int currentHealth, int maxHealth) {
        UpdateHealthBarImage(currentHealth, maxHealth);
    }

    public void UpdatePlayerStaminaBar(float currentStamina, float maxStamina) {
        UpdateStaminaBarImage(currentStamina, maxStamina);
    }

    void UpdateHealthBarImage(int currentHealth, int maxHealth) {
        currentHealth = Mathf.Max(currentHealth, 0);
        maxHealth = Mathf.Max(maxHealth, 1);
        float percent = (float)currentHealth / (float)maxHealth;
        percent = Mathf.Clamp(percent, 0, 1.0f);
        HealthBarImage.transform.localScale = new Vector3(percent, 1, 1);
        HealthBarImage.color = Color.Lerp(EmptyHealthColor, FullHealthColor, percent);
    }

    public void UpdateStaminaBarImage(float currentStamina, float maxStamina) {
        currentStamina = Mathf.Max(currentStamina, 0);
        maxStamina = Mathf.Max(maxStamina, 1);
        float percent = currentStamina / maxStamina;
        percent = Mathf.Clamp(percent, 0, 1.0f);
        StaminaBarImage.transform.localScale = new Vector3(percent, 1, 1);
        StaminaBarImage.color = Color.Lerp(EmptyStaminaColor, FullStaminaColor, percent);
    }
}
