using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerHealth : MonoBehaviour,IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;
    public static Action OnTowerDestroyed;
    public TextMeshProUGUI healthText;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnTowerDestroyed.Invoke();
        }
    }

    private void Update()
    {
        healthText.text = currentHealth.ToString();
    }
}
