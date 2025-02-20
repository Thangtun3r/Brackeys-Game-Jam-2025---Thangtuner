using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthSwitcher : MonoBehaviour
{
    [SerializeField] private Image healthFillImage;

    // References to the health components (dynamically found)
    private PlayerHealth playerHealth;
    private TowerHealth towerHealth;

    // Track which health is currently active; true = PlayerHealth, false = TowerHealth
    private bool usePlayerHealth = true;

    private void OnEnable()
    {
        WaveManager.OnWaveStart += SwitchToTowerHealth;
        WaveManager.OnPhaseOneComplete += SwitchToPlayerHealth;
    }

    private void OnDisable()
    {
        WaveManager.OnWaveStart -= SwitchToTowerHealth;
        WaveManager.OnPhaseOneComplete -= SwitchToPlayerHealth;
    }

    private void Update()
    {
        
        if (usePlayerHealth && playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthBar;
                UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
            }
        }
        else if (!usePlayerHealth && towerHealth == null)
        {
            towerHealth = FindObjectOfType<TowerHealth>();
            if (towerHealth != null)
            {
                towerHealth.OnHealthChanged += UpdateHealthBar;
                UpdateHealthBar(towerHealth.GetCurrentHealth(), towerHealth.GetMaxHealth());
            }
        }
    }

    // Switch to using TowerHealth
    private void SwitchToTowerHealth()
    {
        usePlayerHealth = false;
        Debug.Log("Switched to TowerHealth");
        UpdateHealthBar();
    }

    // Switch to using PlayerHealth
    private void SwitchToPlayerHealth()
    {
        usePlayerHealth = true;
        Debug.Log("Switched to PlayerHealth");
        UpdateHealthBar();
    }

    // Update the health bar using the provided health values
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    // Overloaded method: update the health bar based on the active component
    private void UpdateHealthBar()
    {
        if (usePlayerHealth && playerHealth != null)
        {
            UpdateHealthBar(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
        }
        else if (!usePlayerHealth && towerHealth != null)
        {
            UpdateHealthBar(towerHealth.GetCurrentHealth(), towerHealth.GetMaxHealth());
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthBar;
        }
        if (towerHealth != null)
        {
            towerHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
}
