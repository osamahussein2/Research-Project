using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // In-Game UI elements
    private Slider healthBar;
    private Image emotionalImage;
    
    // Load emotional images
    [SerializeField] public List<Sprite> emotionalImages;

    // Health parameters
    private const float maxHealth = 1.0f;
    private float currentHealth = 0.0f;

    // Emotional image parameters
    private int emotionIndex = 0;

    // Add in-game UI object as a static variable
    static private GameObject inGameUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the in game UI variable to this game object (very unlikely to return a null error)
        inGameUI = gameObject;

        // Find a health bar somewhere in the scene
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();

        // Find an emotional image somewhere in the scene
        emotionalImage = GameObject.Find("EmotionalImage").GetComponent<Image>();

        // Set current health to max health
        currentHealth = maxHealth;

        // Set health bar slider value to current health value
        healthBar.value = currentHealth;

        // Set emotional image sprite to the emotion index of the emotional images list if it's not null
        if (emotionalImage != null)
        {
            emotionalImage.sprite = emotionalImages[emotionIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        PressToUpdateHealthBar(KeyCode.O, KeyCode.P);
        PressToUpdateEmotionalImage(KeyCode.Alpha1, KeyCode.Alpha2);
    }

    private void PressToUpdateHealthBar(KeyCode decreaseHealthKey, KeyCode increaseHealthKey)
    {
        // Decrease health
        if (Input.GetKey(decreaseHealthKey) && currentHealth > 0.0f)
        {
            currentHealth -= Time.deltaTime;
            healthBar.value = currentHealth; // Update slider value too
        }

        // Increae health
        else if (Input.GetKey(increaseHealthKey) && currentHealth < maxHealth)
        {
            currentHealth += Time.deltaTime;
            healthBar.value = currentHealth; // Update slider value too
        }
    }

    private void PressToUpdateEmotionalImage(KeyCode decreaseIndexKey, KeyCode increaseIndexKey)
    {
        // Go to previous emotional image
        if (Input.GetKeyDown(decreaseIndexKey) && emotionIndex > 0)
        {
            emotionIndex -= 1;
            emotionalImage.sprite = emotionalImages[emotionIndex];
        }

        // Go to next emotional image
        else if (Input.GetKeyDown(increaseIndexKey) && emotionIndex < emotionalImages.Count - 1)
        {
            emotionIndex += 1;
            emotionalImage.sprite = emotionalImages[emotionIndex];
        }
    }

    // Public method for getting this game object easily outside this class without needing multiple instances
    static public GameObject GetGameObject()
    {
        return inGameUI;
    }
}
