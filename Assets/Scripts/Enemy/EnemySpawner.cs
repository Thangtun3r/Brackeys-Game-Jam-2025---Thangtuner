using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")] public EnemyObjectPool enemyPool;
    public float spawnInterval = 3f;

    // These are the fixed totals per wave (set by the WaveManager)
    public int poolOneCount = 10; // Total enemies to spawn from pool 1
    public int poolTwoCount = 10; // Total enemies to spawn from pool 2

// Counters for how many enemies have been spawned (never decremented)
    private int spawnedTotalPoolOne = 0;

    private int spawnedTotalPoolTwo = 0;

// Counters for how many enemies are currently active (decremented once each enemy is removed)
    private int activePoolOne = 0;
    private int activePoolTwo = 0;

    private List<Vector3Int> pathToFollow;
    private Vector3Int startTile;
    private bool isSpawning = false;

    public void SetPath(List<Vector3Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: No valid path provided.");
            return;
        }

        pathToFollow = path;
        startTile = path[0];
    }

    public void StartWave(int newPoolOneCount, int newPoolTwoCount)
    {
        if (isSpawning) return;
        poolOneCount = newPoolOneCount;
        poolTwoCount = newPoolTwoCount;
        spawnedTotalPoolOne = 0;
        spawnedTotalPoolTwo = 0;
        activePoolOne = 0;
        activePoolTwo = 0;
        StartCoroutine(SpawnEnemies());
        Debug.Log($"Wave started! Spawning {poolOneCount} from Pool 1 and {poolTwoCount} from Pool 2.");
    }

    IEnumerator SpawnEnemies()
    {
        isSpawning = true;
        while (spawnedTotalPoolOne < poolOneCount || spawnedTotalPoolTwo < poolTwoCount)
        {
            int poolIndex = -1;
            // Choose which pool to spawn from (randomly if both are still below limit)
            if (spawnedTotalPoolOne < poolOneCount && (spawnedTotalPoolTwo >= poolTwoCount || Random.Range(0, 2) == 0))
            {
                poolIndex = 0;
                spawnedTotalPoolOne++;
                activePoolOne++;
            }
            else if (spawnedTotalPoolTwo < poolTwoCount)
            {
                poolIndex = 1;
                spawnedTotalPoolTwo++;
                activePoolTwo++;
            }

            if (poolIndex == -1) break; // safeguard

            GameObject enemy = enemyPool.GetEnemyFromPool(poolIndex);
            if (enemy != null)
            {
                EnemyAIPathFollower enemyAI = enemy.GetComponent<EnemyAIPathFollower>();
                if (enemyAI.baseTilemap == null)
                {
                    enemyAI.baseTilemap = FindObjectOfType<TilemapAStarPathfinder>().overlayTilemap;
                }

                Vector3 spawnPosition = enemyAI.baseTilemap.GetCellCenterWorld(startTile);
                enemy.transform.position = spawnPosition;
                enemyAI.SetPath(pathToFollow, this, poolIndex);
                // Ensure the enemy’s health component knows its pool index
                EnemyHealth health = enemy.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.SetPoolIndex(poolIndex);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    public void EnemyDestroyed(int poolIndex)
    {
        if (poolIndex == 0)
            activePoolOne--;
        else if (poolIndex == 1)
            activePoolTwo--;

        Debug.Log(
            $"Enemy destroyed from pool {poolIndex}. Active counts: Pool1: {activePoolOne}, Pool2: {activePoolTwo}");
        // When both active counts reach zero, notify the WaveManager that Phase One is complete
        if (activePoolOne <= 0 && activePoolTwo <= 0)
        {
            FindObjectOfType<WaveManager>().OnWavePhaseOneComplete();
        }
    }
}