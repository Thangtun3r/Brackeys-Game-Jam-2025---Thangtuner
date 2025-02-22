using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapFMOD : MonoBehaviour
{
    private string soundEventPath = "event:/Swap"; // FMOD event for sound
    
    public void PlaySound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(soundEventPath);
    }
}
