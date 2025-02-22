using UnityEngine;
using DG.Tweening;

public class FloatingHeart : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmount = 0.5f;  // How much it moves up and down
    public float floatDuration = 1f;  // Time to complete one up/down cycle
    public Ease floatEase = Ease.InOutSine; // Smoother movement

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        StartFloating();
    }

    void StartFloating()
    {
        transform.DOMoveY(startPos.y + floatAmount, floatDuration)
            .SetEase(floatEase)
            .SetLoops(-1, LoopType.Yoyo); // Infinite up and down movement
    }
}