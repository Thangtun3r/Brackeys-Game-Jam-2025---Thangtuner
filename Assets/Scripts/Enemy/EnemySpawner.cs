using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")] 
    public EnemyObjectPool enemyPool;
    public float spawnInterval = 3f;

    public int poolOneCount = 10; // Number of enemies from pool 1
    public int poolTwoCount = 10; // Number of enemies from pool 2
    public int currentEnemyCount = 0; // ✅ Add this inside EnemySpawner class


    private int spawnedFromPoolOne = 0;
    private int spawnedFromPoolTwo = 0;
    private List<Vector3Int> pathToFollow;
    private Vector3Int startTile;
    private bool isSpawning = false; // ✅ Prevents multiple coroutines from running

    public void SetPath(List<Vector3Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: No valid path provided.");
            return;
        }

        pathToFollow = path;
        startTile = path[0]; // Set the first tile in the path as the spawn location
    }

    public void StartWave(int newPoolOneCount, int newPoolTwoCount)
    {
        if (isSpawning) return; // ✅ Prevents re-triggering while a wave is still active

        poolOneCount = newPoolOneCount;
        poolTwoCount = newPoolTwoCount;

        spawnedFromPoolOne = 0;
        spawnedFromPoolTwo = 0;

        StartCoroutine(SpawnEnemies());

        Debug.Log($"Wave started! Spawning {poolOneCount} from Pool 1 and {poolTwoCount} from Pool 2.");
    }


    IEnumerator SpawnEnemies()
    {
        isSpawning = true; // ✅ Prevents multiple coroutines from starting
        int totalEnemies = poolOneCount + poolTwoCount;
        int spawnedEnemies = 0; // ✅ Tracks how many enemies have been spawned

        while (spawnedEnemies < totalEnemies) // ✅ Stops exactly when reaching totalEnemies
        {
            int poolIndex = -1;

            if (spawnedFromPoolOne < poolOneCount && (spawnedFromPoolTwo >= poolTwoCount || Random.Range(0, 2) == 0))
            {
                poolIndex = 0;
                spawnedFromPoolOne++;
            }
            else if (spawnedFromPoolTwo < poolTwoCount)
            {
                poolIndex = 1;
                spawnedFromPoolTwo++;
            }

            if (poolIndex == -1) break; // ✅ Exit if all enemies have been spawned

            // Spawn the enemy from the selected pool
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

                spawnedEnemies++; // ✅ Correctly track spawned enemies
                currentEnemyCount++; // ✅ Track active enemies
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false; // ✅ Mark spawning as finished
    }


    public void EnemyDestroyed()
    {
        currentEnemyCount--; // ✅ Decrease only the active enemy count

        if (currentEnemyCount <= 0) // ✅ Check when all active enemies are gone
        {
            FindObjectOfType<WaveManager>().OnWavePhaseOneComplete(); // Notify WaveManager
        }
    }

}
