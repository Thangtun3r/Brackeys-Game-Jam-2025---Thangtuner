using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIPathFollower : MonoBehaviour
{
    [HideInInspector]
    public bool hasBeenCounted = false;
    [Header("References")]
    public Tilemap baseTilemap;
    public float moveSpeed = 2f;

    private List<Vector3> worldWaypoints;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private EnemySpawner spawner;
    private int poolIndex;
    private SpriteRenderer spriteRenderer;

    public EnemySpawner GetSpawner()
    {
        return spawner;
    }

    void Start()
    {
        // Initialize the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        poolIndex = assignedPoolIndex;
    }

    void Update()
    {
        if (!isMoving || worldWaypoints == null || worldWaypoints.Count == 0)
            return;

        if (currentWaypointIndex < worldWaypoints.Count)
        {
            Vector3 targetPos = worldWaypoints[currentWaypointIndex];
        
            // Determine horizontal movement direction
            float diffX = targetPos.x - transform.position.x;
            if (diffX < 0)
                spriteRenderer.flipX = true;  // Flip to face left
            else if (diffX > 0)
                spriteRenderer.flipX = false; // Face right

            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Enemy has reached its destination.
            isMoving = false;
            // Ensure that we only notify the spawner once:
            EnemyHealth health = GetComponent<EnemyHealth>();
            if (health != null && !health.hasBeenCounted)
            {
                health.hasBeenCounted = true;
                spawner.EnemyDestroyed(poolIndex);
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
    }

}
