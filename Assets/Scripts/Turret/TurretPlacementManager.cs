using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurretPlacementManager : MonoBehaviour
{
    public static TurretPlacementManager Instance;

    [Header("Tilemap Settings")]
    public Tilemap allowedTilemap; // Tilemap where turret placement is allowed

    [Header("Preview Settings")]
    public Transform previewHolder; // Container for the turret preview

    private TurretData selectedTurretData;
    private GameObject currentPreview;
    private Dictionary<Vector3Int, GameObject> placedTurrets = new Dictionary<Vector3Int, GameObject>();

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    // When this component is disabled, also remove the preview object
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
            Vector3Int gridPos = allowedTilemap.WorldToCell(mouseWorldPos);
            Vector3 cellCenter = allowedTilemap.GetCellCenterWorld(gridPos);

            // Update preview position
            if (currentPreview != null)
                currentPreview.transform.position = cellCenter;

            bool isValidPlacement = IsTileAvailable(gridPos);

            // Visual feedback using the preview's color (if it has a SpriteRenderer)
            if (currentPreview != null) {
                SpriteRenderer sr = currentPreview.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = isValidPlacement ? Color.green : Color.red;
            }

            // Left-click to place turret
            if (isValidPlacement && Input.GetMouseButtonDown(0)) {
                PlaceTurret(gridPos);
            }
        }

        // Right-click to bulldoze (remove) a turret and refund its cost
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
                Destroy(turretToRemove);
                placedTurrets.Remove(gridPos);
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

        // Optionally, adjust appearance (e.g., semi-transparency) to indicate preview status
        Renderer renderer = currentPreview.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    /// <summary>
    /// Places the turret at the given grid cell if there are enough coins.
    /// </summary>
    private void PlaceTurret(Vector3Int gridPos) {
        if (!ResourceManager.Instance.SpendCoins(selectedTurretData.cost)) {
            Debug.LogWarning("Not enough coins to place turret.");
            return;
        }

        Vector3 cellCenter = allowedTilemap.GetCellCenterWorld(gridPos);
        GameObject newTurret = Instantiate(selectedTurretData.turretPrefab, cellCenter, Quaternion.identity);
        placedTurrets.Add(gridPos, newTurret);

        // Optionally, you can also assign the turretData to a Turret component on the new turret
        Turret turretComponent = newTurret.GetComponent<Turret>();
        if(turretComponent != null) {
            turretComponent.turretData = selectedTurretData;
        }
    }

    /// <summary>
    /// Checks if the grid cell is valid for turret placement.
    /// </summary>
    private bool IsTileAvailable(Vector3Int gridPos) {
        // A cell is available if the allowed tilemap has a tile and no turret is placed there
        return allowedTilemap.HasTile(gridPos) && !placedTurrets.ContainsKey(gridPos);
    }
}
