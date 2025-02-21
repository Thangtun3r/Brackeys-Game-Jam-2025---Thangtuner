using System;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    void OnEnable()
    {
        // Hide the cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure the object remains in the 2D plane

        // Update the object's position to follow the mouse
        transform.position = mousePosition;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }
}