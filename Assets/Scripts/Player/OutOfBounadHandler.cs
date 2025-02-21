using UnityEngine;
using TMPro;

public class OutOfBoundsHandler : MonoBehaviour
{
    [Header("Out-of-Bounds Settings")]
    public LayerMask tilemapLayer; // Assign the Tilemap Layer in the Inspector
    public float timeBeforeHealthDrain = 3f; // Time before health starts draining
    public float healthDrainInterval = 1f; // Time between each health loss
    public int healthDrainAmount = 1; // Health lost per interval

    [Header("UI Warning")]
    public GameObject warningUI; // Assign parent GameObject containing both texts
    public TextMeshProUGUI countdownText; // Assign in the Inspector

    private PlayerHealth playerHealth;
    private bool isOutsideTilemap = false;
    private float outsideTimer = 0f;
    private float healthDrainTimer = 0f;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        // Disable UI at the start
        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }
    }

    void Update()
    {
        CheckTilemapBounds();
    }

    void CheckTilemapBounds()
    {
        // Check if the player is inside the tilemap
        Collider2D tilemapCollider = Physics2D.OverlapPoint(transform.position, tilemapLayer);

        if (tilemapCollider != null)
        {
            // Player is inside tilemap → Reset everything
            if (isOutsideTilemap)
            {
                Debug.Log("Player re-entered the tilemap. Resetting health drain.");
            }

            isOutsideTilemap = false;
            outsideTimer = 0f;
            healthDrainTimer = 0f; // Stop draining health when player is inside

            // Hide UI
            if (warningUI != null)
            {
                warningUI.SetActive(false);
            }
        }
        else
        {
            // Player is outside the tilemap
            if (!isOutsideTilemap)
            {
                Debug.Log("Player left the tilemap. Countdown to health drain started.");
            }

            isOutsideTilemap = true;

            // Show UI
            if (warningUI != null)
            {
                warningUI.SetActive(true);
            }
        }

        // If outside, start countdown
        if (isOutsideTilemap)
        {
            outsideTimer += Time.deltaTime;

            // Update countdown text
            if (countdownText != null)
            {
                float timeLeft = Mathf.Max(0, timeBeforeHealthDrain - outsideTimer);
                countdownText.text = $"Health Drain in: {timeLeft:F1}s";
            }

            // Start health drain after the delay
            if (outsideTimer >= timeBeforeHealthDrain)
            {
                healthDrainTimer += Time.deltaTime;
                if (healthDrainTimer >= healthDrainInterval)
                {
                    playerHealth.TakeDamage(healthDrainAmount);
                    healthDrainTimer = 0f; // Reset health drain timer
                }
            }
        }
    }
}
