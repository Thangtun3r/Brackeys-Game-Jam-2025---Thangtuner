using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulldozeMode : MonoBehaviour
{
    public static bool bulldozeMode = false;
    public GameObject bullDozSprite;
    public GameObject turretButtons;

    private void Update()
    {
        Bulldoze();
    }

    private void Bulldoze()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            bulldozeMode = !bulldozeMode;

            // Toggle the active state of the target object
            if (bullDozSprite != null)
            {
                bullDozSprite.SetActive(!bullDozSprite.activeSelf);
                turretButtons.SetActive(!turretButtons.activeSelf);
            }
        }
    }
}