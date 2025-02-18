using UnityEngine;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject pathfinder;
    public GameObject turretButtonUI;

    private bool isPathfinderActive = true;

    void Start()
    {
        // Ensure only one object is active at the start
        if (pathfinder != null && turretButtonUI != null)
        {
            pathfinder.SetActive(true);
            turretButtonUI.SetActive(false);
        }
    }

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
        if (pathfinder != null && turretButtonUI != null)
        {
            isPathfinderActive = !isPathfinderActive;
            pathfinder.SetActive(isPathfinderActive);
            turretButtonUI.SetActive(!isPathfinderActive);
        }
    }
}