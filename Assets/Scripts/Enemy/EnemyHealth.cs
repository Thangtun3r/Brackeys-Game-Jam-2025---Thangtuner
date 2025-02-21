using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [HideInInspector]
    public bool hasBeenCounted = false;

    [Header("Enemy Health Settings")]
    public float maxHealth = 100;
    private float currentHealth;

    [Header("Enemy AI Settings")] 
    public int coinValue = 1;

    [Header("UI Settings")]
    public Image healthBar; // Assign this in the Inspector

    private EnemyAIPathFollower aiPathFollower;
    private int poolIndex;
    private bool isPartOfWave = false; // Mark if this enemy belongs to the current wave

    public EnemySpawner GetSpawner()
    {
        if (aiPathFollower != null)
            return aiPathFollower.GetSpawner();
        return null;
    }

    void Awake()
    {
        currentHealth = maxHealth;
        aiPathFollower = GetComponent<EnemyAIPathFollower>();
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        UpdateHealthUI(); // Update UI when taking damage

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        ResourceManager.Instance.AddCoins(coinValue);

        if (!hasBeenCounted)
        {
            hasBeenCounted = true;
            EnemySpawner spawner = GetSpawner();
            if (spawner != null && isPartOfWave)
            {
                spawner.EnemyDestroyed(poolIndex);
            }
        }

        EnemyObjectPool enemyPool = FindObjectOfType<EnemyObjectPool>();
        if (enemyPool != null)
        {
            enemyPool.ReturnEnemyToPool(gameObject, poolIndex);
        }
        else
        {
            Debug.LogWarning("EnemyObjectPool not found in scene!");
        }
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isPartOfWave = true;
        hasBeenCounted = false;
        UpdateHealthUI();
    }

    public void SetPoolIndex(int index)
    {
        poolIndex = index;
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}
