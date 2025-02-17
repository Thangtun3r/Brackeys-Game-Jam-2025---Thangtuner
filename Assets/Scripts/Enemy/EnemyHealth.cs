using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Enemy Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private EnemyAIPathFollower aiPathFollower;
    private EnemySpawner spawner;

    public EnemySpawner GetSpawner()
    {
        return spawner;
    }

    void Awake()
    {
        currentHealth = maxHealth;
        aiPathFollower = GetComponent<EnemyAIPathFollower>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed!");

        // Get reference to EnemyAIPathFollower
        EnemyAIPathFollower aiPathFollower = GetComponent<EnemyAIPathFollower>();
        if (aiPathFollower != null)
        {
            // Use the public getter to retrieve the spawner
            EnemySpawner spawner = aiPathFollower.GetSpawner();
            if (spawner != null)
            {
                spawner.EnemyDestroyed();
            }
        }

        // Return enemy to object pool instead of destroying
        FindObjectOfType<EnemyObjectPool>().ReturnEnemyToPool(gameObject);
    }


    // Reset health when reactivated from the pool
    private void OnEnable()
    {
        currentHealth = maxHealth;
    }
}