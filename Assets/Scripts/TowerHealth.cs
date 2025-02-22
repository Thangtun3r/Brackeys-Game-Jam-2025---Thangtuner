using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TowerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public static Action OnTowerDestroyed;
    public event Action<float, float> OnHealthChanged; // Added event for UI updates
    public TextMeshProUGUI healthText;
    public string gameOverScene = "GameOver"; // Add the name of the scene to load on death

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
            Die();
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        OnTowerDestroyed?.Invoke();
        SceneManager.LoadScene(gameOverScene); // Load the specified scene
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}