using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : MonoBehaviour
{
    [Header("Pooling Settings")]
    public GameObject[] enemyPrefabs; // Array of enemy prefabs
    public int poolSize = 10;

    private Dictionary<int, Queue<GameObject>> enemyPools = new Dictionary<int, Queue<GameObject>>();

    void Start()
    {
        InitializePools();
    }

    void InitializePools()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            enemyPools[i] = new Queue<GameObject>();

            for (int j = 0; j < poolSize; j++)
            {
                GameObject enemy = Instantiate(enemyPrefabs[i]);
                enemy.SetActive(false);
                enemyPools[i].Enqueue(enemy);
            }
        }
    }

    public GameObject GetEnemyFromPool(int poolIndex)
    {
        if (enemyPools.ContainsKey(poolIndex) && enemyPools[poolIndex].Count > 0)
        {
            GameObject enemy = enemyPools[poolIndex].Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            Debug.LogWarning($"Enemy Pool {poolIndex} exhausted! Consider increasing pool size.");
            return null;
        }
    }

    public void ReturnEnemyToPool(GameObject enemy, int poolIndex)
    {
        enemy.SetActive(false);
        enemyPools[poolIndex].Enqueue(enemy);
    }
}