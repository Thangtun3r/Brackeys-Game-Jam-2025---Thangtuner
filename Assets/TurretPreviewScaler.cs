using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TurretPreviewScaler : MonoBehaviour
{
    [SerializeField] CanvasScaler mainCanvasScaler;
    RectTransform previewRect;

    Vector3 originalScale;

    void Awake()
    {
        previewRect = GetComponent<RectTransform>();
        originalScale = previewRect.localScale;
    }

    void LateUpdate()
    {
        // If the CanvasScaler uses 'Scale With Screen Size,' we can replicate its scaling logic:
        //    scaleFactor = pow( screenWidth / refWidth, (1 - matchWidthOrHeight) )
        //                 * pow( screenHeight / refHeight, matchWidthOrHeight )
        //
        // Or simply read 'mainCanvasScaler.scaleFactor' if it’s correct for your setup.

        float scaleFactor = mainCanvasScaler.scaleFactor;  // easiest approach
        previewRect.localScale = originalScale / scaleFactor;
    }
}