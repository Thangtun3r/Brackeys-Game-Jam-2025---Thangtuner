using UnityEngine;

public class TurretBase : MonoBehaviour
{
    // Set these in the Inspector for each turret prefab.
    // When the turret is placed, it is fully refundable.
    public int normalRefundCost = 100;      // Refund amount when turret is refundable.
    public int nonRefundableRefundCost = 50;  // Refund amount when turret is flagged nonrefundable.
    
    // This flag indicates whether the turret is still fully refundable.
    public bool isRefundable = true;
}