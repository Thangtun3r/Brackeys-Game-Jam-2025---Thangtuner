using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretPlacementManager : MonoBehaviour
{
    public static TurretPlacementManager Instance;

    [Header("Tilemap Settings")]
    public Tilemap allowedTilemap; // Tilemap where turret placement is allowed

    [Header("Path Settings")]
    public Tilemap pathTilemap;    // Tilemap where the path is painted
    public Tile pathTile;          // The tile used for the painted path

    [Header("Preview Settings")]
    public Transform previewHolder; // Container for the turret preview

    private TurretData selectedTurretData;
    private GameObject currentPreview;
    // Dictionary mapping each grid cell to the turret occupying it
    private Dictionary<Vector3Int, GameObject> placedTurrets = new Dictionary<Vector3Int, GameObject>();

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public bool IsCellOccupied(Vector3Int cellPos)
    {
        return placedTurrets.ContainsKey(cellPos);
    }

    
    // Remove the preview object when this component is disabled
    private void OnDisable() {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }
    }

    private void Update() {
        // Handle turret placement if a turret is selected
        if (selectedTurretData != null) {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Get the cell under the mouse – treat it as the bottom-left of a 2x2 area.
            Vector3Int gridPos = allowedTilemap.WorldToCell(mouseWorldPos);

            // Calculate the center of the bottom-left cell…
            Vector3 cellCenter = allowedTilemap.GetCellCenterWorld(gridPos);
            // …then offset by half the cell size to get the center of the 2x2 block.
            Vector3 offset = allowedTilemap.layoutGrid.cellSize / 2f;
            Vector3 placementCenter = cellCenter + offset;

            // Update the preview's position
            if (currentPreview != null)
                currentPreview.transform.position = placementCenter;

            // Check if all four cells in the 2x2 block are available
            bool isValidPlacement = IsAreaAvailable(gridPos);

            // Provide visual feedback using the preview's color (if it has a SpriteRenderer)
            if (currentPreview != null) {
                SpriteRenderer sr = currentPreview.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = isValidPlacement ? Color.green : Color.red;
            }

            // Left-click to place turret if the area is valid
            if (isValidPlacement && Input.GetMouseButtonDown(0)) {
                PlaceTurret(gridPos, placementCenter);
            }
        }

        // Right-click to remove (bulldoze) a turret and refund its cost
        if (Input.GetMouseButtonDown(1)) {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = allowedTilemap.WorldToCell(mouseWorldPos);
            if (placedTurrets.ContainsKey(gridPos)) {
                GameObject turretToRemove = placedTurrets[gridPos];
                Turret turretComponent = turretToRemove.GetComponent<Turret>();
                if (turretComponent != null && turretComponent.turretData != null) {
                    ResourceManager.Instance.AddCoins(turretComponent.turretData.cost);
                    Debug.Log($"Refunded {turretComponent.turretData.cost} coins.");
                }
                // Remove the turret from all occupied cells
                List<Vector3Int> keysToRemove = new List<Vector3Int>();
                foreach (var kvp in placedTurrets)
                {
                    if (kvp.Value == turretToRemove)
                        keysToRemove.Add(kvp.Key);
                }
                foreach (var key in keysToRemove)
                    placedTurrets.Remove(key);

                Destroy(turretToRemove);
            }
        }
    }

    /// <summary>
    /// Called by the UI to select a turret type for placement.
    /// </summary>
    public void SetSelectedTurret(TurretData turretData) {
        selectedTurretData = turretData;

        // Remove any existing preview
        if (currentPreview != null)
            Destroy(currentPreview);

        // Instantiate a preview using the turret prefab and parent it to the previewHolder
        currentPreview = Instantiate(turretData.turretPrefab, previewHolder);
        // Disable colliders and scripts on the preview so it doesn't interfere with gameplay
        Collider col = currentPreview.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        foreach (MonoBehaviour mb in currentPreview.GetComponents<MonoBehaviour>())
            mb.enabled = false;

        // Adjust appearance (e.g., semi-transparency) to indicate preview status
        Renderer renderer = currentPreview.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    /// <summary>
    /// Places the turret at the given grid cell (bottom-left of 2x2 area) if there are enough coins.
    /// </summary>
    private void PlaceTurret(Vector3Int gridPos, Vector3 placementCenter) {
        if (!ResourceManager.Instance.SpendCoins(selectedTurretData.cost)) {
            Debug.LogWarning("Not enough coins to place turret.");
            return;
        }

        // Instantiate the turret at the center of the 2x2 area
        GameObject newTurret = Instantiate(selectedTurretData.turretPrefab, placementCenter, Quaternion.identity);
        
        // Optionally, assign turretData to the turret component
        Turret turretComponent = newTurret.GetComponent<Turret>();
        if(turretComponent != null) {
            turretComponent.turretData = selectedTurretData;
        }

        // Mark all cells occupied by the 2x2 turret as taken
        Vector3Int[] occupiedCells = new Vector3Int[4] {
            gridPos,
            gridPos + new Vector3Int(1, 0, 0),
            gridPos + new Vector3Int(0, 1, 0),
            gridPos + new Vector3Int(1, 1, 0)
        };
        foreach (var cell in occupiedCells)
        {
            placedTurrets.Add(cell, newTurret);
        }
    }

    /// <summary>
    /// Checks if the 2x2 area starting at the given grid cell is valid for turret placement.
    /// It now also checks that none of the cells are painted with the path tile.
    /// </summary>
    private bool IsAreaAvailable(Vector3Int gridPos) {
        // Define the four cells of the 2x2 block
        Vector3Int[] requiredCells = new Vector3Int[4] {
            gridPos,
            gridPos + new Vector3Int(1, 0, 0),
            gridPos + new Vector3Int(0, 1, 0),
            gridPos + new Vector3Int(1, 1, 0)
        };

        foreach (var cell in requiredCells) {
            // The area is invalid if any cell is missing a tile, is already occupied, 
            // or is painted with the path tile.
            if (!allowedTilemap.HasTile(cell) || placedTurrets.ContainsKey(cell))
                return false;

            if (pathTilemap != null && pathTilemap.GetTile(cell) == pathTile)
                return false;
        }
        return true;
    }
}
