using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "TowerDefense/TurretData")]
public class TurretData : ScriptableObject
{
    public GameObject turretPrefab;
    public Sprite previewSprite;

    [Header("Costs")]
    public int originalCost = 100;         // The cost to buy the turret (and if fully refundable)
    public int partialRefundCost = 50;     // Manually assigned partial refund
    [HideInInspector] public int cost;     // The actual cost used at bulldoze time

    [Header("Stats")]
    public float range;
    public float fireRate;

    [Header("Refund Settings")]
    public bool isRefundable = true;

    private void OnEnable()
    {
        // Whenever this ScriptableObject is loaded or reloaded, reset cost to originalCost by default
        cost = originalCost;
    }
}