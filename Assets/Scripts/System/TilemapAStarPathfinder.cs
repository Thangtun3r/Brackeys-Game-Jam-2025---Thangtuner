using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAStarPathfinder : MonoBehaviour
{
    [Header("Tilemap & Tile Settings")]
    public Tilemap overlayTilemap;
    public Tile pathTile;

    [Header("Allowed Area")]
    public Tilemap allowedAreaTilemap;

    [Header("Path Requirements")]
    public int requiredPathLength = 5;

    [Header("Spawner Reference")]
    public EnemySpawner enemySpawner;

    private List<Vector3Int> paintedCells = new List<Vector3Int>();
    public event Action OnPathValidated;
    public static bool pathValid = false;
    public static bool placeAble = false;

    void Update()
    {
        HandleMouseInput();
        ValidateAndStartPath();
    }

    void HandleMouseInput()
    {
        if (placeAble)
        {
            if (Input.GetMouseButton(0))
            {
                if (paintedCells.Count >= requiredPathLength)
                    return;

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPos = overlayTilemap.WorldToCell(worldPos);

                if (IsWithinAllowedArea(cellPos) && !IsCellPainted(cellPos) && !IsCellOccupiedByTurret(cellPos))
                {
                    paintedCells.Add(cellPos);
                    overlayTilemap.SetTile(cellPos, pathTile);

                    if (Has2x2Block(paintedCells))
                    {
                        overlayTilemap.SetTile(cellPos, null);
                        paintedCells.Remove(cellPos);
                        return;
                    }

                    if (HasIntersection(paintedCells))
                    {
                        overlayTilemap.SetTile(cellPos, null);
                        paintedCells.Remove(cellPos);
                    }
                }
            }

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
            if (GetNeighborCount(cell, cellSet) == 1)
            {
                start = cell;
                break;
            }
        }

        List<Vector3Int> ordered = new List<Vector3Int> { start };
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
            if (set.Contains(cell + Vector3Int.right) && set.Contains(cell + Vector3Int.up) && set.Contains(cell + new Vector3Int(1, 1, 0)))
                return true;
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

    void ValidateAndStartPath()
    {
        if (paintedCells.Count != requiredPathLength || Has2x2Block(paintedCells) || HasIntersection(paintedCells))
        {
            pathValid = false;
        }
        else
        {
            pathValid = true;
            enemySpawner.SetPath(OrderPaintedCells(paintedCells));
        }
    }
}
