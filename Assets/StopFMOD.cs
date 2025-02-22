using UnityEngine;
using UnityEngine.SceneManagement;

public class StopAllFMODSoundsOnSceneChange : MonoBehaviour
{
    private FMOD.Studio.Bus masterBus;

    void Start()
    {
        // Get the Master Bus ("/" represents the root bus)
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Suspending FMOD Mixer...");
        FMODUnity.RuntimeManager.CoreSystem.mixerSuspend();
        FMODUnity.RuntimeManager.CoreSystem.mixerResume();
    }


    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}