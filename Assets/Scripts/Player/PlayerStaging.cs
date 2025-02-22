using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerStaging : MonoBehaviour
{
    public float delayTime;
    public GameObject vcam;
    public MonoBehaviour playerMovement; // Reference to the player movement script
    public TilemapAStarPathfinder pathfinder; // Reference to the A* pathfinder script

    private void OnEnable()
    {
        playerMovement.enabled = false;
        // Check if the pathfinder reference is set
        if (pathfinder != null && pathfinder.startTileObject != null)
        {
            // Move the player to the starting tile's position from the A* pathfinder.
            transform.position = pathfinder.startTileObject.transform.position;
        }
        
        StartCoroutine(startStaging());
        vcam.SetActive(true);
    }

    private void OnDisable()
    {
        gameObject.tag = "Untagged";
    }

    IEnumerator startStaging()
    {
        yield return new WaitForSeconds(2f);
        vcam.SetActive(false);
        yield return new WaitForSeconds(delayTime);
        playerMovement.enabled = true;
        gameObject.tag = "Damageable";
    }
}

