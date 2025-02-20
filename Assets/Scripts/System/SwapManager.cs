using System.Collections;
using UnityEngine;

public class SwapManager : MonoBehaviour
{
    public float swapDelay;
    void OnEnable()
    {
        // Subscribe to events
        WaveManager.OnPhaseOneComplete += EnableChildren;
        WaveManager.OnWaveEnd += DisableChildren;
    }

    void OnDisable()
    {
        // Unsubscribe from events to prevent memory leaks
        WaveManager.OnPhaseOneComplete -= EnableChildren;
        WaveManager.OnWaveEnd -= DisableChildren;
    }

    /// <summary>
    /// Begins the coroutine to enable all child objects after a delay.
    /// </summary>
    private void EnableChildren()
    {
        Debug.Log("Phase One ended. Enabling all children after a delay.");
        StartCoroutine(EnableChildrenAfterDelay());
    }

    /// <summary>
    /// Coroutine that waits for the specified delay before enabling children.
    /// </summary>
    private IEnumerator EnableChildrenAfterDelay()
    {
        yield return new WaitForSeconds(swapDelay);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Immediately disables all child objects.
    /// </summary>
    private void DisableChildren()
    {
        Debug.Log("Wave ended. Disabling all children.");
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}