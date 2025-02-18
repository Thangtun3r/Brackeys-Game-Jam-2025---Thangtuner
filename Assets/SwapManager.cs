using UnityEngine;

public class SwapManager : MonoBehaviour
{
    void OnEnable()
    {
        // ✅ Subscribe to Phase One and Wave End events
        WaveManager.OnPhaseOneComplete += EnableChildren;
        WaveManager.OnWaveEnd += DisableChildren;
    }

    void OnDisable()
    {
        // ✅ Unsubscribe to prevent memory leaks
        WaveManager.OnPhaseOneComplete -= EnableChildren;
        WaveManager.OnWaveEnd -= DisableChildren;
    }

    /// <summary>
    /// Enables all child objects when Phase One ends.
    /// </summary>
    private void EnableChildren()
    {
        Debug.Log("Phase One ended. Enabling all children.");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true); // ✅ Enable each child object
        }
    }

    /// <summary>
    /// Disables all child objects when the Wave ends.
    /// </summary>
    private void DisableChildren()
    {
        Debug.Log("Wave ended. Disabling all children.");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // ✅ Disable each child object
        }
    }
}