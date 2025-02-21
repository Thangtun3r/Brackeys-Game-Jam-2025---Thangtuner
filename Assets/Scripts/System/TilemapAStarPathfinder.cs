using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class TilemapAStarPathfinder : MonoBehaviour
{
    [Header("Error UI Animator")] 
    public Animator errorAnimator; // Assign in the Inspector

    [Header("Tilemap & Tile Settings")] 
    public Tilemap overlayTilemap;
    public Tile pathTile;

    [Header("Allowed Area")] 
    public Tilemap allowedAreaTilemap;

    [Header("Path Requirements")] 
    public int requiredPathLength = 5;

    [Header("Spawner Reference")] 
    public EnemySpawner enemySpawner;

    [Header("UI Elements")] 
    public TMP_Text errorText; // UI text element for error messages
    public TMP_Text tileCountText; // Displays current tile count / required tiles

    [Header("Tower Health Object")] 
    public GameObject towerHealthObject;

    [Header("Start Tile Object")] 
    public GameObject startTileObject;

    [Header("Particle Effects")]
    // Particle effect to play when a tile is placed.
    public ParticleSystem spawnEffect;

    // Particle effect to play when a tile is removed.
    public ParticleSystem removeEffect;

    private List<Vector3Int> paintedCells = new List<Vector3Int>();
    public event Action OnPathValidated;
    public static bool pathValid = false;
    public static bool placeAble = false;

    // Directions for adjacency checks
    private static readonly Vector3Int[] Directions =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    void Update()
    {
        HandleMouseInput();
        ValidateAndStartPath();

        // Update the tile count display (current/required)
        if (tileCountText != null)
            tileCountText.text = paintedCells.Count.ToString() + " / " + requiredPathLength.ToString();
    }

    void HandleMouseInput()
    {
        if (!placeAble)
            return;

        // Left-click: Attempt to place a tile only if one is not already placed at that cell.
        if (Input.GetMouseButton(0))
        {
            if (paintedCells.Count >= requiredPathLength)
                return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = overlayTilemap.WorldToCell(worldPos);

            if (IsWithinAllowedArea(cellPos) && !IsCellPainted(cellPos) && !IsCellOccupiedByTurret(cellPos))
            {
                // Create a temporary list simulating the addition.
                List<Vector3Int> tempCells = new List<Vector3Int>(paintedCells);
                tempCells.Add(cellPos);

                // Check for invalid configurations.
                if (Has2x2Block(tempCells))
                {
                    ShowError("Invalid: creates a 2x2 block.");
                    return;
                }

                if (HasIntersection(tempCells))
                {
                    ShowError("Invalid: creates an intersection.");
                    return;
                }

                if (!IsContiguous(tempCells))
                {
                    ShowError("Invalid: path is not contiguous.");
                    return;
                }

                // All checks passed – clear errors, add the tile, update tilemap.
                ClearError();
                paintedCells.Add(cellPos);
                overlayTilemap.SetTile(cellPos, pathTile);

                // Trigger spawn particle effect at the tile's center.
                if (spawnEffect != null)
                {
                    Vector3 cellCenter = overlayTilemap.GetCellCenterWorld(cellPos);
                    ParticleSystem effect = Instantiate(spawnEffect, cellCenter, Quaternion.identity);
                    effect.Play();
                    Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
                }
            }
        }

        // Right-click: Remove a tile if one is placed.
        if (Input.GetMouseButton(1))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = overlayTilemap.WorldToCell(worldPos);

            if (IsWithinAllowedArea(cellPos) && IsCellPainted(cellPos))
            {
                overlayTilemap.SetTile(cellPos, null);
                paintedCells.Remove(cellPos);
                ClearError();

                // Trigger removal particle effect at the tile's center.
                if (removeEffect != null)
                {
                    Vector3 cellCenter = overlayTilemap.GetCellCenterWorld(cellPos);
                    ParticleSystem effect = Instantiate(removeEffect, cellCenter, Quaternion.identity);
                    effect.Play();
                    Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
                }
            }
        }
    }

    void ValidateAndStartPath()
    {
        // Validate only if painted cells equal required length and no invalid configurations.
        if (paintedCells.Count != requiredPathLength || Has2x2Block(paintedCells) || HasIntersection(paintedCells))
        {
            pathValid = false;
            if (towerHealthObject != null)
                towerHealthObject.SetActive(false);
            if (startTileObject != null)
                startTileObject.SetActive(false);
            return;
        }

        if (!IsContiguous(paintedCells))
        {
            pathValid = false;
            ShowError("Invalid final path: not contiguous.");
            if (towerHealthObject != null)
                towerHealthObject.SetActive(false);
            if (startTileObject != null)
                startTileObject.SetActive(false);
            return;
        }

        // Valid path.
        pathValid = true;
        ClearError();

        List<Vector3Int> ordered = OrderPaintedCells(paintedCells);

        // Set path tiles (the start and end could use different tiles if needed).
        if (ordered.Count > 0)
        {
            for (int i = 1; i < ordered.Count - 1; i++)
            {
                overlayTilemap.SetTile(ordered[i], pathTile);
            }
        }

        enemySpawner.SetPath(ordered);

        // Position and enable the start tile object.
        if (startTileObject != null)
        {
            if (ordered.Count > 0)
            {
                Vector3 startPointWorldPos = overlayTilemap.GetCellCenterWorld(ordered[0]);
                startTileObject.transform.position = startPointWorldPos;
                startTileObject.SetActive(true);
            }
            else
            {
                startTileObject.SetActive(false);
            }
        }

        // Position and enable the tower health object at the endpoint.
        if (towerHealthObject != null)
        {
            if (ordered.Count > 0)
            {
                Vector3 endpointWorldPos = overlayTilemap.GetCellCenterWorld(ordered[ordered.Count - 1]);
                towerHealthObject.transform.position = endpointWorldPos;
                towerHealthObject.SetActive(true);
            }
            else
            {
                towerHealthObject.SetActive(false);
            }
        }
    }

    bool IsCellOccupiedByTurret(Vector3Int cellPos)
    {
        return TurretPlacementManager.Instance != null && TurretPlacementManager.Instance.IsCellOccupied(cellPos);
    }

    bool IsWithinAllowedArea(Vector3Int cellPos)
    {
        return allowedAreaTilemap.GetTile(cellPos) != null;
    }

    bool IsCellPainted(Vector3Int cellPos)
    {
        Tile currentTile = overlayTilemap.GetTile(cellPos) as Tile;
        return currentTile == pathTile;
    }

    bool Has2x2Block(List<Vector3Int> cells)
    {
        HashSet<Vector3Int> set = new HashSet<Vector3Int>(cells);
        foreach (Vector3Int cell in cells)
        {
            if (set.Contains(cell + Vector3Int.right) &&
                set.Contains(cell + Vector3Int.up) &&
                set.Contains(cell + new Vector3Int(1, 1, 0)))
            {
                return true;
            }
        }

        return false;
    }

    bool HasIntersection(List<Vector3Int> cells)
    {
        HashSet<Vector3Int> set = new HashSet<Vector3Int>(cells);
        foreach (Vector3Int cell in cells)
        {
            if (GetNeighborCount(cell, set) > 2)
                return true;
        }

        return false;
    }

    // Check contiguity using a BFS.
    bool IsContiguous(List<Vector3Int> cells)
    {
        if (cells.Count <= 1) return true;

        HashSet<Vector3Int> set = new HashSet<Vector3Int>(cells);
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(cells[0]);
        visited.Add(cells[0]);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            foreach (Vector3Int dir in Directions)
            {
                Vector3Int neighbor = current + dir;
                if (set.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return visited.Count == cells.Count;
    }

    int GetNeighborCount(Vector3Int cell, HashSet<Vector3Int> cells)
    {
        int count = 0;
        foreach (Vector3Int dir in Directions)
        {
            if (cells.Contains(cell + dir))
                count++;
        }

        return count;
    }

    List<Vector3Int> OrderPaintedCells(List<Vector3Int> cells)
    {
        if (cells.Count == 0)
            return new List<Vector3Int>();

        HashSet<Vector3Int> cellSet = new HashSet<Vector3Int>(cells);
        Vector3Int start = cells[0];

        // Choose a starting cell with only one neighbor if available.
        foreach (Vector3Int c in cells)
        {
            if (GetNeighborCount(c, cellSet) == 1)
            {
                start = c;
                break;
            }
        }

        List<Vector3Int> ordered = new List<Vector3Int> { start };
        cellSet.Remove(start);
        Vector3Int current = start;

        while (cellSet.Count > 0)
        {
            bool foundNext = false;
            foreach (Vector3Int dir in Directions)
            {
                Vector3Int neighbor = current + dir;
                if (cellSet.Contains(neighbor))
                {
                    ordered.Add(neighbor);
                    cellSet.Remove(neighbor);
                    current = neighbor;
                    foundNext = true;
                    break;
                }
            }

            if (!foundNext) break;
        }

        return ordered;
    }

    void ShowError(string msg)
    {
        if (errorText != null)
            errorText.text = msg;

        // If we have an animator, trigger the error animation
        if (errorAnimator != null)
            errorAnimator.SetBool("isError", true);
        StartCoroutine(swithcOffAnimation());
    }

    void ClearError()
    {
        if (errorText != null)
            errorText.text = "";
    }

    IEnumerator swithcOffAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        if (errorAnimator != null)
            errorAnimator.SetBool("isError", false);
    }
}
