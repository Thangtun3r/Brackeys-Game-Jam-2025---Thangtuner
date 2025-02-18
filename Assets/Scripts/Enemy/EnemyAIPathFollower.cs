using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIPathFollower : MonoBehaviour
{
    [Header("References")]
    public Tilemap baseTilemap;
    public float moveSpeed = 2f;

    private List<Vector3> worldWaypoints;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private EnemySpawner spawner;
    private int poolIndex; // Store the pool index

    public EnemySpawner GetSpawner()
    {
        return spawner;
    }

    public void SetPath(List<Vector3Int> pathCells, EnemySpawner assignedSpawner, int assignedPoolIndex)
    {
        if (pathCells == null || pathCells.Count == 0)
        {
            Debug.LogWarning("EnemyAI: No path provided.");
            return;
        }

        worldWaypoints = new List<Vector3>();
        foreach (Vector3Int cell in pathCells)
        {
            worldWaypoints.Add(baseTilemap.GetCellCenterWorld(cell));
        }

        transform.position = worldWaypoints[0];
        currentWaypointIndex = 1;
        isMoving = true;
        spawner = assignedSpawner;
        poolIndex = assignedPoolIndex; // Store the pool index
    }

    void Update()
    {
        if (!isMoving || worldWaypoints == null || worldWaypoints.Count == 0)
            return;

        if (currentWaypointIndex < worldWaypoints.Count)
        {
            Vector3 targetPos = worldWaypoints[currentWaypointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Enemy reached destination → Return to Pool
            isMoving = false;
            spawner.EnemyDestroyed();

            EnemyObjectPool enemyPool = FindObjectOfType<EnemyObjectPool>();
            if (enemyPool != null)
            {
                enemyPool.ReturnEnemyToPool(gameObject, poolIndex); // Now passing poolIndex correctly
            }
            else
            {
                Debug.LogWarning("EnemyObjectPool not found in scene!");
            }
        }
    }
}
