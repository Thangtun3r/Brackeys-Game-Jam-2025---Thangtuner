using System;
using UnityEngine;
using TMPro;

public class TowerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public static Action OnTowerDestroyed;
    public event Action<float, float> OnHealthChanged; // Added event for UI updates
    public TextMeshProUGUI healthText;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnTowerDestroyed?.Invoke();
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}