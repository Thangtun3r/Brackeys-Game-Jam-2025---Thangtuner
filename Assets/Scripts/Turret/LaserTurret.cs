using System.Collections;
using UnityEngine;

public class LaserBeamAuto : MonoBehaviour, IDamageable
{
    [Header("External References")]
    public Transform shootingPoint;
    public LineRenderer lineRenderer;

    [Header("Damage / Multiplier")]
    public float baseDamage = 5f;
    public float multiplierIncrement = 0.2f;
    public float timeToIncreaseMultiplier = 3f;
    public float maxMultiplier = 2f;
    public float damageInterval = 1f;
    public float maxDistance = 50f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    [Header("Overheat System")]
    public float maxOverheat = 10f;
    public float overheatIncrease = 1f;
    public float normalCoolingRate = 1f;
    public float overheatedCooldownTime = 5f;
    public float overheatingCooldownPenalty = 3f;

    [Header("Laser Width Settings")]
    public float baseLineWidth = 0.05f;
    public float widthPerMultiplier = 0.02f;

    [Header("Health Settings")]
    public float health = 100f;
    // Assign your reward prefab in the Inspector.
    public GameObject rewardPrefab;

    private Transform currentTarget = null;
    private float currentMultiplier = 1f;
    private float damageTimer = 0f;
    private float multiplierTimer = 0f;
    private float overheatMeter = 0f;
    private bool isOverheated = false;
    private bool isFiring = false;

    void Start()
    {
        // Optional global setting: Now raycasts won't hit 2D triggers at all
        Physics2D.queriesHitTriggers = false; 

        if (lineRenderer)
        {
            lineRenderer.enabled = false;
            // Force the line in front of your sprites:
            // Use an actual Sorting Layer name from your project:
            lineRenderer.sortingLayerName = "Foreground"; 
            lineRenderer.sortingOrder = 10;
        }
    }

    void Update()
    {
        FindAndTrackTarget();

        // If we have a valid target and are NOT overheated, we fire
        if (!isOverheated && currentTarget != null)
        {
            StartFiring();
        }
        else
        {
            StopFiring();
        }

        HandleOverheat();
    }

    void FindAndTrackTarget()
    {
        bool lostTarget = false;
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            lostTarget = true;
        }
        else
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            if (dist > maxDistance)
                lostTarget = true;
        }

        if (lostTarget)
        {
            currentTarget = null;
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Damageable");
            float shortestDistance = Mathf.Infinity;
            GameObject nearestTarget = null;

            foreach (GameObject t in targets)
            {
                if (!t.activeInHierarchy) continue;

                float distance = Vector2.Distance(transform.position, t.transform.position);
                if (distance < shortestDistance && distance <= maxDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = t;
                }
            }

            if (nearestTarget != null)
            {
                currentTarget = nearestTarget.transform;
            }
        }

        if (currentTarget != null)
        {
            Vector3 direction = currentTarget.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void StartFiring()
    {
        if (!isFiring)
        {
            isFiring = true;
            if (lineRenderer) lineRenderer.enabled = true;
        }

        FireLaserBeam();
        UpdateDamageMultiplier();
    }

    void StopFiring()
    {
        if (isFiring)
        {
            isFiring = false;
            if (lineRenderer) lineRenderer.enabled = false;
            ResetMultiplier();
        }
    }

    void FireLaserBeam()
    {
        if (!lineRenderer) return;

        // Adjust the width based on multiplier
        float width = baseLineWidth + (currentMultiplier - 1f) * widthPerMultiplier;
        width = Mathf.Max(0f, width);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        lineRenderer.SetPosition(0, shootingPoint.position);

        // Raycast (ignoring triggers thanks to queriesHitTriggers=false)
        RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingPoint.right, maxDistance);
        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);
            IDamageable dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null)
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    float totalDamage = baseDamage * currentMultiplier;
                    dmg.TakeDamage(totalDamage);
                    damageTimer = 0f;
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(1, shootingPoint.position + shootingPoint.right * maxDistance);
        }
    }

    void UpdateDamageMultiplier()
    {
        multiplierTimer += Time.deltaTime;
        if (multiplierTimer >= timeToIncreaseMultiplier)
        {
            currentMultiplier += multiplierIncrement;
            currentMultiplier = Mathf.Min(currentMultiplier, maxMultiplier);
            multiplierTimer = 0f;
        }
    }

    void HandleOverheat()
    {
        if (isFiring)
        {
            overheatMeter += overheatIncrease * Time.deltaTime;
            if (overheatMeter >= maxOverheat)
            {
                StartCoroutine(OverheatCooldown());
            }
        }
        else
        {
            overheatMeter -= normalCoolingRate * Time.deltaTime;
            overheatMeter = Mathf.Max(overheatMeter, 0f);
        }
    }

    IEnumerator OverheatCooldown()
    {
        isOverheated = true;
        if (lineRenderer) lineRenderer.enabled = false;
        isFiring = false;
        ResetMultiplier();

        float penaltyTime = (overheatMeter > maxOverheat) ? overheatingCooldownPenalty : 0f;
        yield return new WaitForSeconds(overheatedCooldownTime + penaltyTime);

        isOverheated = false;
        overheatMeter = 0f;
    }

    void ResetMultiplier()
    {
        currentMultiplier = 1f;
        multiplierTimer = 0f;
        damageTimer = 0f;
    }

    // IDamageable implementation
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);
        }
        gameObject.SetActive(false); // Simulate destruction.
    }
}
