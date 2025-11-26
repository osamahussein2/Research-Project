using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // In-Game UI elements
    private Slider healthBar;

    // Health parameters
    private const float maxHealth = 1.0f;
    private float currentHealth = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find a health bar somewhere in the scene
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();

        // Set current health to max health
        currentHealth = maxHealth;

        // Set health bar slider value to current health value
        healthBar.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        PressToUpdateHealthBar(KeyCode.O, KeyCode.P);
    }

    private void PressToUpdateHealthBar(KeyCode decreaseHealthKey, KeyCode increaseHealthKey)
    {
        if (Input.GetKey(decreaseHealthKey) && currentHealth > 0.0f)
        {
            currentHealth -= Time.deltaTime;
            healthBar.value = currentHealth;
        }

        else if (Input.GetKey(increaseHealthKey) && currentHealth < maxHealth)
        {
            currentHealth += Time.deltaTime;
            healthBar.value = currentHealth;
        }
    }
}
