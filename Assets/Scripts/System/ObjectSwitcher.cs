using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject turretButtonUI; // ✅ Assign in Inspector
    private bool isPathfinderActive = true; // ✅ Tracks the original state
    public GameObject requiredTileUI;

    void OnEnable()
    {
        // ✅ Subscribe to static events from WaveManager
        WaveManager.OnWaveStart += DisablePathfinder;
        WaveManager.OnWaveEnd += EnablePathfinder;
    }

    void OnDisable()
    {
        // ✅ Unsubscribe when disabled/destroyed to prevent memory leaks
        WaveManager.OnWaveStart -= DisablePathfinder;
        WaveManager.OnWaveEnd -= EnablePathfinder;
    }

    void Update()
    {
        // Check if Tab key is pressed and only allow toggling when a wave is NOT active
        if (Input.GetKeyDown(KeyCode.Tab) && isPathfinderActive)
        {
            ToggleObjects();
        }
    }

    void ToggleObjects()
    {
        // Toggle placeAble state
        TilemapAStarPathfinder.placeAble = !TilemapAStarPathfinder.placeAble;

        // Toggle UI visibility based on the new state
        if (turretButtonUI != null)
        {
            turretButtonUI.SetActive(!TilemapAStarPathfinder.placeAble);
        }

        // Ensure requiredTileUI also follows the toggle logic
        if (requiredTileUI != null)
        {
            requiredTileUI.SetActive(TilemapAStarPathfinder.placeAble && isPathfinderActive);
        }
    }


    /// <summary>
    /// Disables pathfinder and prevents toggling during a wave.
    /// </summary>
    void DisablePathfinder()
    {
        isPathfinderActive = false;
        TilemapAStarPathfinder.placeAble = false; // ✅ Prevent placement
        if (turretButtonUI != null) turretButtonUI.SetActive(false); // ✅ Hide UI
        requiredTileUI.SetActive(false);

        Debug.Log("Pathfinder disabled: Wave started.");
    }

    /// <summary>
    /// Re-enables pathfinder and allows toggling when the wave ends.
    /// </summary>
    void EnablePathfinder()
    {
        isPathfinderActive = true;
        TilemapAStarPathfinder.placeAble = true; // ✅ Allow placement again
    
        if (turretButtonUI != null) 
            turretButtonUI.SetActive(true); // ✅ Show UI

        // Ensure requiredTileUI is NOT enabled when the wave ends
        if (requiredTileUI != null) 
            requiredTileUI.SetActive(false); 

        Debug.Log("Pathfinder enabled: Wave ended.");
    }

}
