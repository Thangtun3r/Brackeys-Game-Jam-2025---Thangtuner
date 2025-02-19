using UnityEngine;
using UnityEngine.UIElements; // UI Toolkit

public class LosingScreen : MonoBehaviour
{
    public Image _image;

    void OnEnable()
    {
        TowerHealth.OnTowerDestroyed += ShowLosingScreen;
        PlayerHealth.OnPlayerDeath += ShowLosingScreen;
    }

    void Start()
    {
        _image = GetComponent<Image>();
    }

    void ShowLosingScreen()
    {
        _image.style.opacity = 1f; // 1 = 100% visible
        Debug.Log("You lost!");
    }

    void OnDestroy()
    {
        TowerHealth.OnTowerDestroyed -= ShowLosingScreen;
        PlayerHealth.OnPlayerDeath -= ShowLosingScreen;
    }
}