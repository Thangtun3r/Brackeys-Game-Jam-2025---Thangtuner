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
    
    private void OnDisable() {
        if (currentPreview != null) {
            currentPreview.SetActive(false);
        }
    }

    private void OnEnable() {
        if (currentPreview != null) {
            currentPreview.SetActive(true);
        }
    }

    private void Update() {
        if (!BulldozeMode.bulldozeMode)
        {
            if (selectedTurretData != null) {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = allowedTilemap.WorldToCell(mouseWorldPos);
                Vector3 cellCenter = allowedTilemap.GetCellCenterWorld(gridPos);
                Vector3 offset = allowedTilemap.layoutGrid.cellSize / 2f;
                Vector3 placementCenter = cellCenter + offset;

                if (currentPreview != null)
                    currentPreview.transform.position = placementCenter;

                bool isValidPlacement = IsAreaAvailable(gridPos);
                
                if (currentPreview != null) {
                    SpriteRenderer sr = currentPreview.GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.color = isValidPlacement ? Color.green : Color.red;
                }

                if (isValidPlacement && Input.GetMouseButtonDown(0)) {
                    PlaceTurret(gridPos, placementCenter);
                }
            }
        }
        else // Bulldoze mode
        {
            Destroy(currentPreview);
            currentPreview = null;
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = allowedTilemap.WorldToCell(mouseWorldPos);
                if (placedTurrets.ContainsKey(gridPos))
                {
                    GameObject turretToRemove = placedTurrets[gridPos];
                    TurretBase refundComponent = turretToRemove.GetComponent<TurretBase>();
                    if (refundComponent != null)
                    {
                        int refundAmount = refundComponent.isRefundable 
                            ? refundComponent.normalRefundCost 
                            : refundComponent.nonRefundableRefundCost;
                        ResourceManager.Instance.AddCoins(refundAmount);
                        Debug.Log("Refunded " + refundAmount + " coins from turret: " + turretToRemove.name);
                    }
                    else
                    {
                        Debug.LogWarning("No TurretBase component found on turret: " + turretToRemove.name);
                    }
                    List<Vector3Int> keysToRemove = new List<Vector3Int>();
                    foreach (var kvp in placedTurrets)
                    {
                        if (kvp.Value == turretToRemove)
                            keysToRemove.Add(kvp.Key);
                    }
                    foreach (var key in keysToRemove)
                        placedTurrets.Remove(key);

                    Destroy(turretToRemove);
                    
                    // **Sound plays when turret is bulldozed**
                    FMODUnity.RuntimeManager.PlayOneShot("event:/PlaceTiles", transform.position);
                }
            }
        }
    }

    public void SetSelectedTurret(TurretData turretData) {
        selectedTurretData = turretData;

        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(turretData.turretPrefab, previewHolder);
        Collider col = currentPreview.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        foreach (MonoBehaviour mb in currentPreview.GetComponents<MonoBehaviour>())
            mb.enabled = false;

        Renderer renderer = currentPreview.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    private void PlaceTurret(Vector3Int gridPos, Vector3 placementCenter) {
        if (!ResourceManager.Instance.SpendCoins(selectedTurretData.cost)) {
            Debug.LogWarning("Not enough coins to place turret.");
            return;
        }

        GameObject newTurret = Instantiate(selectedTurretData.turretPrefab, placementCenter, Quaternion.identity);
        
        // **Sound only plays when turret is actually placed**
        FMODUnity.RuntimeManager.PlayOneShot("event:/PlaceTiles", transform.position);

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

    private bool IsAreaAvailable(Vector3Int gridPos) {
        Vector3Int[] requiredCells = new Vector3Int[4] {
            gridPos,
            gridPos + new Vector3Int(1, 0, 0),
            gridPos + new Vector3Int(0, 1, 0),
            gridPos + new Vector3Int(1, 1, 0)
        };

        foreach (var cell in requiredCells) {
            if (!allowedTilemap.HasTile(cell) || placedTurrets.ContainsKey(cell))
                return false;
            if (pathTilemap != null && pathTilemap.GetTile(cell) == pathTile)
                return false;
        }
        return true;
    }
}