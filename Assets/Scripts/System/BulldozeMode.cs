using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulldozeMode : MonoBehaviour
{
    public static bool bulldozeMode = false;
    public Animator bullDozAnimator; // Reference to the Animator
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

            // Toggle animator's isBulldoze parameter instead of setting active/inactive
            if (bullDozAnimator != null)
            {
                bullDozAnimator.SetBool("isBulldoze", bulldozeMode);
            }

            // Toggle turret buttons visibility
            if (turretButtons != null)
            {
                turretButtons.SetActive(!turretButtons.activeSelf);
            }
        }
    }
}