using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements; // UI Toolkit

public class LosingScreen : MonoBehaviour
{
    public string sceneToLoad = "MainMenu";

    void OnEnable()
    {
        TowerHealth.OnTowerDestroyed += ShowLosingScreen;
        PlayerHealth.OnPlayerDeath += ShowLosingScreen;
    }
    
    void ShowLosingScreen()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnDestroy()
    {
        TowerHealth.OnTowerDestroyed -= ShowLosingScreen;
        PlayerHealth.OnPlayerDeath -= ShowLosingScreen;
    }
}