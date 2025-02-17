using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public EnemyObjectPool enemyPool;
    public float spawnInterval = 3f;
    public int maxEnemies = 20;

    private int currentEnemyCount = 0;
    private List<Vector3Int> pathToFollow;
    private Vector3Int startTile; // The starting tile where enemies should spawn

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

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

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (currentEnemyCount < maxEnemies && pathToFollow != null)
            {
                GameObject enemy = enemyPool.GetEnemyFromPool();
                if (enemy != null)
                {
                    EnemyAIPathFollower enemyAI = enemy.GetComponent<EnemyAIPathFollower>();

                    // 🚨 FIX: Ensure baseTilemap is assigned before using it
                    if (enemyAI.baseTilemap == null)
                    {
                        enemyAI.baseTilemap = FindObjectOfType<TilemapAStarPathfinder>().overlayTilemap;
                    }

                    // Convert the start tile to world position
                    Vector3 spawnPosition = enemyAI.baseTilemap.GetCellCenterWorld(startTile);
                    enemy.transform.position = spawnPosition; // Spawn enemy at the start tile

                    // Assign the path to the enemy
                    enemyAI.SetPath(pathToFollow, this);

                    currentEnemyCount++;
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void EnemyDestroyed()
    {
        currentEnemyCount--;
    }
}