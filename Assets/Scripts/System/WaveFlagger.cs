using UnityEngine;

public class WaveFlagger : MonoBehaviour
{
    // Call this method when the new wave is about to start.
    public void StartWave()
    {
        FlagAllExistingTurretsAsNonRefundable();
        Debug.Log("Wave started: Existing turrets are now nonrefundable.");
        // ... any other wave logic (spawning enemies, etc.)
    }

    private void FlagAllExistingTurretsAsNonRefundable()
    {
        // Find all turret objects that have the TurretBase component.
        TurretBase[] turrets = FindObjectsOfType<TurretBase>();
        foreach (TurretBase turret in turrets)
        {
            if (turret.isRefundable)
            {
                turret.isRefundable = false;
                Debug.Log("Turret " + turret.gameObject.name + " flagged as nonrefundable.");
            }
        }
    }
}