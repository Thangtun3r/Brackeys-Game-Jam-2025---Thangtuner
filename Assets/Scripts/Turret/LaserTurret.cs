using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LaserBeamAuto : MonoBehaviour, IDamageable
{
    [Header("External References")]
    public Transform shootingPoint;
    public LineRenderer lineRenderer;
    public ParticleSystem deadParticle;
    public GameObject healthUI;
    public GameObject overHeatIndicator;

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
    public float maxHealth;
    public float health = 100f;
    public GameObject rewardPrefab;

    [Header("Health UI")]
    public Image healthBar; // Assign in the Inspector

    private Transform currentTarget = null;
    private Transform lastTarget = null;
    private float currentMultiplier = 1f;
    private float damageTimer = 0f;
    private float multiplierTimer = 0f;
    private float overheatMeter = 0f;
    private bool isOverheated = false;
    private bool isFiring = false;

    void Start()
    {
        deadParticle.Stop();
        Physics2D.queriesHitTriggers = false; 

        if (lineRenderer)
        {
            lineRenderer.enabled = false;
            lineRenderer.sortingLayerName = "Foreground"; 
            lineRenderer.sortingOrder = 10;
        }

        if (overHeatIndicator != null)
        {
            overHeatIndicator.SetActive(false);
        }

        UpdateHealthUI();
    }

    void Update()
    {
        FindAndTrackTarget();

        if (currentTarget != lastTarget)
        {
            ResetMultiplier();
            lastTarget = currentTarget;
        }

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

        float width = baseLineWidth + (currentMultiplier - 1f) * widthPerMultiplier;
        width = Mathf.Max(0f, width);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        lineRenderer.SetPosition(0, shootingPoint.position);
        

// Perform the raycast with the layer mask.
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
        if (overHeatIndicator != null)
        {
            overHeatIndicator.SetActive(true);
        }

        float penaltyTime = (overheatMeter > maxOverheat) ? overheatingCooldownPenalty : 0f;
        yield return new WaitForSeconds(overheatedCooldownTime + penaltyTime);

        isOverheated = false;
        overheatMeter = 0f;
        
        overHeatIndicator.SetActive(false);
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
        UpdateHealthUI(); // Updates health bar when taking damage

        if (health <= 0f)
        {
            deadParticle.Play();
            Die();
        }
    }

    void Die()
    {
        if (rewardPrefab != null)
        {
            Instantiate(rewardPrefab, transform.position, Quaternion.identity);
            healthUI.SetActive(false);
        }
        gameObject.SetActive(false); // Simulate destruction.
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }
}
