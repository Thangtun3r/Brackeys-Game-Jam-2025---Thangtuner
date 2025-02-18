using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject turretButtonUI; // ✅ Assign UI button in the Inspector

    private bool isPathfinderActive = true;

    void Update()
    {
        // Check if Tab key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
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
}