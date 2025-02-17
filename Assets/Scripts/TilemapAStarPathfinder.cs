using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAStarPathfinder : MonoBehaviour
{
    [Header("Tilemap & Tile Settings")]
    public Tilemap overlayTilemap;   // Tilemap where the player paints the path.
    public Tile pathTile;            // Tile used for painting the path.

    [Header("Allowed Area")]
    public Tilemap allowedAreaTilemap; // Defines the placeable area (cells with a tile here are allowed).

    [Header("Path Requirements")]
    public int requiredPathLength = 5; // Required number of painted tiles.

    [Header("Spawner Reference")]
    public EnemySpawner enemySpawner; // Reference to Enemy Spawner.

    // List to record all painted cells.
    private List<Vector3Int> paintedCells = new List<Vector3Int>();

    void Update()
    {
        HandleMouseInput();

        // Press space to validate and start the pathfinding process.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ValidateAndStartPath();
        }
    }

    void HandleMouseInput()
    {
        // Left mouse button: paint while held.
        if (Input.GetMouseButton(0))
        {
            // Enforce maximum paint length.
            if (paintedCells.Count >= requiredPathLength)
            {
                Debug.Log("Cannot paint: required path length reached.");
                return;
            }
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = overlayTilemap.WorldToCell(worldPos);

            if (IsWithinAllowedArea(cellPos) && !IsCellPainted(cellPos))
            {
                // Temporarily add the tile for checking.
                paintedCells.Add(cellPos);
                overlayTilemap.SetTile(cellPos, pathTile);

                // Check if this move created a 2x2 block.
                if (Has2x2Block(paintedCells))
                {
                    // Undo the placement and notify the player.
                    overlayTilemap.SetTile(cellPos, null);
                    paintedCells.Remove(cellPos);
                    Debug.Log("Cannot paint: placing this tile creates a 2x2 block.");
                    return;
                }

                // Check if this move creates an intersection.
                if (HasIntersection(paintedCells))
                {
                    // Undo the placement and notify the player.
                    overlayTilemap.SetTile(cellPos, null);
                    paintedCells.Remove(cellPos);
                    Debug.Log("Cannot paint: placing this tile would create an intersection.");
                }
            }
        }

        // Right mouse button: erase while held.
        if (Input.GetMouseButton(1))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = overlayTilemap.WorldToCell(worldPos);
            if (IsWithinAllowedArea(cellPos) && IsCellPainted(cellPos))
            {
                overlayTilemap.SetTile(cellPos, null);
                paintedCells.Remove(cellPos);
            }
        }
    }

    bool IsWithinAllowedArea(Vector3Int cellPos)
    {
        return allowedAreaTilemap.GetTile(cellPos) != null;
    }

    bool IsCellPainted(Vector3Int cellPos)
    {
        return overlayTilemap.GetTile(cellPos) == pathTile;
    }

    List<Vector3Int> OrderPaintedCells(List<Vector3Int> cells)
    {
        if (cells.Count == 0)
            return new List<Vector3Int>();

        HashSet<Vector3Int> cellSet = new HashSet<Vector3Int>(cells);
        Vector3Int start = cells[0];

        foreach (Vector3Int cell in cells)
        {
            int neighborCount = GetNeighborCount(cell, cellSet);
            if (neighborCount == 1)
            {
                start = cell;
                break;
            }
        }

        List<Vector3Int> ordered = new List<Vector3Int>();
        ordered.Add(start);
        cellSet.Remove(start);
        Vector3Int current = start;
        while (cellSet.Count > 0)
        {
            bool foundNext = false;
            foreach (Vector3Int dir in new Vector3Int[] { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down })
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

    int GetNeighborCount(Vector3Int cell, HashSet<Vector3Int> cells)
    {
        int count = 0;
        foreach (Vector3Int dir in new Vector3Int[] { Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down })
        {
            if (cells.Contains(cell + dir))
                count++;
        }
        return count;
    }

    bool Has2x2Block(List<Vector3Int> cells)
    {
        HashSet<Vector3Int> set = new HashSet<Vector3Int>(cells);
        foreach (Vector3Int cell in cells)
        {
            Vector3Int right = cell + Vector3Int.right;
            Vector3Int up = cell + Vector3Int.up;
            Vector3Int upRight = cell + new Vector3Int(1, 1, 0);
            if (set.Contains(right) && set.Contains(up) && set.Contains(upRight))
                return true;
        }
        return false;
    }

    bool HasIntersection(List<Vector3Int> cells)
    {
        HashSet<Vector3Int> set = new HashSet<Vector3Int>(cells);
        foreach (Vector3Int cell in cells)
        {
            int neighborCount = GetNeighborCount(cell, set);
            if (neighborCount > 2)
                return true; // More than 2 neighbors = intersection.
        }
        return false;
    }

    void ValidateAndStartPath()
    {
        List<string> errors = new List<string>();

        if (paintedCells.Count != requiredPathLength)
            errors.Add($"Path length is {paintedCells.Count}, but required is {requiredPathLength}.");

        List<Vector3Int> orderedPath = OrderPaintedCells(paintedCells);

        if (Has2x2Block(orderedPath))
            errors.Add("The painted path contains a 2x2 block. Fix this before continuing.");

        if (HasIntersection(orderedPath))
            errors.Add("The painted path crosses itself, which is not allowed.");

        if (errors.Count > 0)
        {
            foreach (string err in errors)
            {
                Debug.Log("Invalid placing: " + err);
            }
        }
        else
        {
            Debug.Log("Path valid. Assigning to spawner.");
            enemySpawner.SetPath(orderedPath);
        }
    }
}
