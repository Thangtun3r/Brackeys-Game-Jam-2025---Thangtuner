using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnSceneChange : MonoBehaviour
{
    private string startingSceneName;

    private void Awake()
    {
        // Remember which scene we started in
        startingSceneName = SceneManager.GetActiveScene().name;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
    {
        // Destroy only if a different scene is loaded
        if (newScene.name != startingSceneName)
        {
            Destroy(gameObject);
        }
    }
}