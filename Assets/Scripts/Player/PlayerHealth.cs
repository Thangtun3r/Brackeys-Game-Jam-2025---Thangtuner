using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;         // Only if you still need TextMeshPro references
using System.Collections;
using FMODUnity;

public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public static Action OnPlayerDeath;
    public event Action<float, float> OnHealthChanged; // Health change event

    [Header("UI Shake Settings")]
    [Tooltip("Assign the RectTransform of the UI element you want to shake.")]
    public RectTransform uiToShake;
    public float shakeDuration = 0.5f;       // How long the shake lasts
    public float shakeMagnitude = 10f;       // Offset magnitude of the shake

    [Header("FMOD Settings")]
    [SerializeField]
    private string damageSoundEventPath;     // FMOD event path for damage sound

    // Track the original anchored position for the UI element.
    private Vector2 originalUIPosition;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        currentHealth = maxHealth; // Initialize health

        if (uiToShake != null)
            originalUIPosition = uiToShake.anchoredPosition;
    }

    private void OnEnable()
    {
        // Ensure UI updates when player is enabled.
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        // Notify any listeners that health changed
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        // Notify listeners of health change
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // 1. Play FMOD one-shot damage sound
        if (!string.IsNullOrEmpty(damageSoundEventPath))
        {
            RuntimeManager.PlayOneShot(damageSoundEventPath);
        }

        // 2. Trigger UI shake
        if (uiToShake != null)
        {
            // If a shake is already in progress, stop it
            if (shakeRoutine != null)
                StopCoroutine(shakeRoutine);

            shakeRoutine = StartCoroutine(ShakeUI());
        }

        // 3. Check if the player is dead
        if (currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    // Coroutine that shakes the assigned UI element
    private IEnumerator ShakeUI()
    {
        float elapsed = 0f;
        // Make sure we capture the original position each time in case the UI element moves
        Vector2 startPos = uiToShake.anchoredPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            uiToShake.anchoredPosition = startPos + new Vector2(offsetX, offsetY);

            yield return null;
        }

        // Reset the position
        uiToShake.anchoredPosition = startPos;
        shakeRoutine = null;
    }
}
