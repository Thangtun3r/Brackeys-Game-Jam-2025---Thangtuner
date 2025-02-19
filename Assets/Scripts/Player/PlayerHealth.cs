using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable,IHealable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public static Action OnPlayerDeath;


    public delegate void HealthChanged(float currentHealth);
    public event HealthChanged OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
    }
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");
        if (currentHealth <= 0)
        {
            OnPlayerDeath.Invoke();
        }
    }

    
}