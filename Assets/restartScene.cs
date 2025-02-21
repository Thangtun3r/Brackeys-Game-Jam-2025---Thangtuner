using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class restartScene : MonoBehaviour
{
    public string sceneToLoad = "Gameplay";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            BackToGameplay();
        }
    }

    private void BackToGameplay()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
