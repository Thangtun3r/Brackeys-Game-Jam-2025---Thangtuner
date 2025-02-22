using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulldozeMode : MonoBehaviour
{
    public static bool bulldozeMode = false;
    public GameObject bullDozeUI;
    public GameObject turretButtons;

    private void Update()
    {
        Bulldoze();
    }

    private void Start()
    {
        bullDozeUI.SetActive(false);
    }

    private void Bulldoze()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/ClickSound");
            bulldozeMode = !bulldozeMode;

            bullDozeUI.SetActive(!bullDozeUI.activeSelf);

            if (turretButtons != null)
            {
                turretButtons.SetActive(!turretButtons.activeSelf);
            }
        }
    }
}