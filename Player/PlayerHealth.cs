using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    public int maxMind = 100;
    public int currentMind;
    public float mindDecreaseRate = 1f; // Mind points lost per second

    void Start()
    {
        currentHealth = maxHealth;
        currentMind = maxMind;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    void Update()
    {
        DecreaseMindOverTime();
    }

    void DecreaseMindOverTime()
    {
        if (currentMind > 0)
        {
            currentMind -= Mathf.RoundToInt(mindDecreaseRate * Time.deltaTime);
            currentMind = Mathf.Clamp(currentMind, 0, maxMind);

            if (currentMind == 0)
            {
                OnMindDepleted();
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;
    }

    public void RestoreMind(int amount)
    {
        currentMind += amount;
        currentMind = Mathf.Clamp(currentMind, 0, maxMind);
    }

    void OnMindDepleted()
    {
        // Implement behavior when mind is fully depleted
        Debug.Log("Mind fully depleted. Player is going insane!");
        // Example: Reduce player's movement speed, alter controls, etc.
    }

    void Die()
    {
        // Implement player death logic
        Debug.Log("Player died");
    }
}

