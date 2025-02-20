using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public static Action OnPlayerDeath;

    public event Action<float, float> OnHealthChanged; // Health change event

    private void Awake()
    {
        currentHealth = maxHealth; // Initialize health
    }

    private void OnEnable()
    {
        // Ensure the UI updates when the player becomes active
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}