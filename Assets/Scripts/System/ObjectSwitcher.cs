using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject turretButtonUI; // ✅ Assign in Inspector
    private bool isPathfinderActive = true; // ✅ Tracks the original state

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
        // Toggle the `placeAble` state in TilemapAStarPathfinder
        TilemapAStarPathfinder.placeAble = !TilemapAStarPathfinder.placeAble;

        // Toggle UI visibility
        if (turretButtonUI != null)
        {
            turretButtonUI.SetActive(!TilemapAStarPathfinder.placeAble);
        }

        Debug.Log($"Toggled: placeAble = {TilemapAStarPathfinder.placeAble}, UI Active = {!TilemapAStarPathfinder.placeAble}");
    }

    /// <summary>
    /// Disables pathfinder and prevents toggling during a wave.
    /// </summary>
    void DisablePathfinder()
    {
        isPathfinderActive = false;
        TilemapAStarPathfinder.placeAble = false; // ✅ Prevent placement
        if (turretButtonUI != null) turretButtonUI.SetActive(false); // ✅ Hide UI

        Debug.Log("Pathfinder disabled: Wave started.");
    }

    /// <summary>
    /// Re-enables pathfinder and allows toggling when the wave ends.
    /// </summary>
    void EnablePathfinder()
    {
        isPathfinderActive = true;
        TilemapAStarPathfinder.placeAble = true; // ✅ Allow placement again
        if (turretButtonUI != null) turretButtonUI.SetActive(true); // ✅ Show UI

        Debug.Log("Pathfinder enabled: Wave ended.");
    }
}
