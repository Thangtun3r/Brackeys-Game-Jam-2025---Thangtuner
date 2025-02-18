using UnityEngine;

public class PreWaveEvent : MonoBehaviour
{
    void Start()
    {
        // ✅ Ensure children are enabled at the start
        EnableChildren();
    }

    void OnEnable()
    {
        // ✅ Subscribe to wave start and wave end events
        WaveManager.OnWaveStart += DisableChildren;
        WaveManager.OnWaveEnd += EnableChildren;
    }

    void OnDisable()
    {
        // ✅ Unsubscribe to prevent memory leaks
        WaveManager.OnWaveStart -= DisableChildren;
        WaveManager.OnWaveEnd -= EnableChildren;
    }

    /// <summary>
    /// Disables all child objects when the wave starts.
    /// </summary>
    private void DisableChildren()
    {
        Debug.Log("Wave started. Disabling PreWaveEvent children.");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false); // ✅ Disable each child object
        }
    }

    /// <summary>
    /// Enables all child objects when the wave ends.
    /// </summary>
    private void EnableChildren()
    {
        Debug.Log("Wave ended. Enabling PreWaveEvent children.");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true); // ✅ Enable each child object
        }
    }
}