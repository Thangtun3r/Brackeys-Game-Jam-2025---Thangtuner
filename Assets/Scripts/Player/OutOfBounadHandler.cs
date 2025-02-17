using UnityEngine;

public class OutOfBoundsHandler : MonoBehaviour
{
    [Header("Out-of-Bounds Settings")]
    public LayerMask tilemapLayer; // Assign the Tilemap Layer in the Inspector
    public float timeBeforeHealthDrain = 3f; // Time before health starts draining
    public float healthDrainInterval = 1f; // Time between each health loss
    public int healthDrainAmount = 1; // Health lost per interval

    private PlayerHealth playerHealth;
    private bool isOutsideTilemap = false;
    private float outsideTimer = 0f;
    private float healthDrainTimer = 0f;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
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
        }
        else
        {
            // Player is outside the tilemap
            if (!isOutsideTilemap) 
            {
                Debug.Log("Player left the tilemap. Countdown to health drain started.");
            }

            isOutsideTilemap = true;
        }

        // If outside, start countdown
        if (isOutsideTilemap)
        {
            outsideTimer += Time.deltaTime;

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
