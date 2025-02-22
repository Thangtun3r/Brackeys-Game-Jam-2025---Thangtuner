using System.Collections;
using UnityEngine;
using TMPro;

public class OutOfBoundsHandler : MonoBehaviour
{
    [Header("Health Drain Settings")]
    public LayerMask pathTileLayer;
    public float drainDelay = 3f;
    public float drainInterval = 1f;
    public float damagePerTick = 10f;
   
    [Header("Warning UI")]
    public TextMeshProUGUI warningText;       
    
    private PlayerHealth playerHealth;         // Reference to the player’s health component.
    private bool isOffPath = false;              // Whether the child object is off the pathtile.
    private float timer = 0f;                    // Countdown timer.
    private bool draining = false;               // Whether the drain coroutine is running.

    void Awake()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    void OnEnable()
    {
        ResetDrainState();
    }

    void ResetDrainState()
    {
        timer = 0f;
        isOffPath = false;
        draining = false;
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isOffPath)
        {
            timer += Time.deltaTime;
        
            if (warningText != null)
            {
                if (timer >= drainDelay)
                {
                    warningText.text = "RETURN TO SAFE PATH NOW! HEALTH IS DRAINING...";
                }
                else
                {
                    float remaining = Mathf.Max(drainDelay - timer, 0f);
                    warningText.text = "RETURN TO SAFE PATH NOW! HEALTH DRAIN IN : " + remaining.ToString("F1") + "s";
                }
                warningText.gameObject.SetActive(true);
            }
        
            // If the delay has elapsed and we're not already draining, start health drain.
            if (timer >= drainDelay && !draining)
            {
                StartCoroutine(DrainHealth());
            }
        }
    }

    IEnumerator DrainHealth()
    {
        draining = true;
        while (isOffPath)
        {
            // Drain the player's health.
            playerHealth.TakeDamage(damagePerTick);
            yield return new WaitForSeconds(drainInterval);
        }
        draining = false;
    }

    // For 2D collisions; if you're using 3D, use OnTriggerEnter/Exit (without the 2D suffix).
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider belongs to a pathtile.
        if (((1 << collision.gameObject.layer) & pathTileLayer) != 0)
        {
            // Player re-enters the path: stop draining and reset the timer.
            isOffPath = false;
            timer = 0f;
            if (warningText != null)
                warningText.gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // When exiting a pathtile, mark the state as off the path and reset the timer.
        if (((1 << collision.gameObject.layer) & pathTileLayer) != 0)
        {
            isOffPath = true;
            timer = 0f;
        }
    }
}
