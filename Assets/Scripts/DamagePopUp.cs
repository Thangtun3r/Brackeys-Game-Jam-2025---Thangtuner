using TMPro;
using UnityEngine;
using DG.Tweening;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    private float moveDistance = 1.5f;
    private float moveDuration = 0.5f;
    private float fadeDuration = 0.8f;
    private float scalePunch = 1.2f;

    public void Setup(int damageAmount, bool isCritical = false)
    {
        textMesh.text = damageAmount.ToString();

        if (isCritical)
        {
            textMesh.color = Color.yellow; // Change to a bold color for crits
            textMesh.fontSize *= 1.5f; // Increase font size for emphasis
        }

        // Start the animation
        Animate();
    }

    private void Animate()
    {
        // Move upwards
        transform.DOMoveY(transform.position.y + moveDistance, moveDuration).SetEase(Ease.OutQuad);

        // Scale punch effect (bounces slightly)
        transform.DOScale(scalePunch, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // Restore to normal size
            transform.DOScale(1f, 0.2f);
        });

        // Fade out
        textMesh.DOFade(0, fadeDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Destroy(gameObject); // Destroy after fadeout
        });
    }
}