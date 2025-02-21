using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitcher : MonoBehaviour
{
    // Assign these in the Unity Inspector
    public GameObject TowerUI;
    public GameObject PlayerUI;
    public GameObject UISwap;
    public Animator UISwapAnimator;
    public Animator swapAnimator;
    public float delay;

    // Keeps track of which UI is currently active
    private bool isTowerActive = true;

    private void OnEnable()
    {
        WaveManager.OnPhaseOneComplete += ShowPlayerUI;
        WaveManager.OnWaveEnd += ShowTowerUI;
    }

    private void OnDisable()
    {
        WaveManager.OnPhaseOneComplete -= ShowPlayerUI;
        WaveManager.OnWaveEnd -= ShowTowerUI;
    }

    void Start()
    {
        ShowTowerUI();
    }
    
    

    public void SwitchUI()
    {
        if (isTowerActive)
        {
            ShowPlayerUI();
        }
        else
        {
            ShowTowerUI();
        }
    }

    private void ShowTowerUI()
    {
        UISwapAnimator.SetBool("SlideToTower", true);
        UISwapAnimator.SetBool("SlideToPlayer", false);
        
        TowerUI.SetActive(true);
        PlayerUI.SetActive(false);
        isTowerActive = true;
    }

    private void ShowPlayerUI()
    {
        swapAnimator.SetBool("swap", true);
        
        StartCoroutine(StartAnimation());
        
        TowerUI.SetActive(false);
        PlayerUI.SetActive(true);
        isTowerActive = false;
    }
    
    private IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds(delay);
        
        UISwapAnimator.SetBool("SlideToTower", false);
        UISwapAnimator.SetBool("SlideToPlayer", true);
        swapAnimator.SetBool("swap", false);
    }
    
    
}