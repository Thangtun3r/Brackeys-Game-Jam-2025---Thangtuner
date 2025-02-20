using UnityEngine;

public class EnforceResolution : MonoBehaviour
{
    public int targetWidth = 1920;
    public int targetHeight = 1080;
    public bool isFullscreen = false; // Set to true if you want fullscreen

    void Start()
    {
        // Set the resolution at startup
        SetResolution();
    }

    void Update()
    {
        // Constantly enforce resolution to prevent user changes
        if (Screen.width != targetWidth || Screen.height != targetHeight || Screen.fullScreen != isFullscreen)
        {
            SetResolution();
        }
    }

    void SetResolution()
    {
        Screen.SetResolution(targetWidth, targetHeight, isFullscreen);
        Debug.Log($"Resolution Enforced: {targetWidth}x{targetHeight}, Fullscreen: {isFullscreen}");
    }
}