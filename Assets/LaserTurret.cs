using System.Collections;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public Transform shootingPoint; // Assign in Inspector
    public LineRenderer lineRenderer; // Assign in Inspector
    public float baseDamage = 5f;
    public float multiplierIncrement = 0.2f;
    public float timeToIncreaseMultiplier = 3f;
    public float maxMultiplier = 2f;
    public float maxDistance = 50f;
    public float damageInterval = 1f;
    public float rotationSpeed = 5f;

    [Header("Overheat System")]
    public float maxOverheat = 10f; // Maximum overheat threshold
    public float overheatIncrease = 1f; // Overheat increases per second
    public float normalCoolingRate = 1f; // Cooling rate when not overheated
    public float overheatedCooldownTime = 5f; // Cooldown time if overheated
    public float overheatingCooldownPenalty = 3f; // Additional cooldown penalty when overheated

    private float currentMultiplier = 1f;
    private float damageTimer = 0f;
    private float multiplierTimer = 0f;
    private IDamageable lastTarget;
    private Transform currentTarget;
    private float overheatMeter = 0f;
    private bool isOverheated = false;
    private bool isFiring = false;

    void Update()
    {
        FindAndTrackTarget();

        if (!isOverheated)
        {
            FireLaser();
        }

        HandleOverheat();
    }

    void FindAndTrackTarget()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            ResetTarget();
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Damageable");
            float shortestDistance = Mathf.Infinity;
            GameObject nearestTarget = null;

            foreach (GameObject target in targets)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestTarget = target;
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

    void FireLaser()
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootingPoint.position);
        isFiring = true;

        RaycastHit2D hit = Physics2D.Raycast(shootingPoint.position, shootingPoint.right, maxDistance);
        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();

            if (damageable != null)
            {
                if (damageable != lastTarget)
                {
                    lastTarget = damageable; // New target assigned, but multiplier does NOT reset
                }

                // Damage interval check
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    float totalDamage = baseDamage * currentMultiplier;
                    damageable.TakeDamage(totalDamage);
                    damageTimer = 0f;
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(1, shootingPoint.position + shootingPoint.right * maxDistance);
        }

        // Multiplier continues even if no hit, only stops if overheated or disabled
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
            overheatMeter = Mathf.Max(overheatMeter, 0);
        }
    }

    IEnumerator OverheatCooldown()
    {
        isOverheated = true;
        lineRenderer.enabled = false;
        isFiring = false;
        yield return new WaitForSeconds(overheatedCooldownTime + (overheatMeter > maxOverheat ? overheatingCooldownPenalty : 0));
        isOverheated = false;
        overheatMeter = 0;
        ResetMultiplier();
    }

    void ResetTarget()
    {
        currentTarget = null;
        transform.rotation = Quaternion.identity;
    }

    void ResetMultiplier()
    {
        currentMultiplier = 1f;
        multiplierTimer = 0f;
        damageTimer = 0f;
        lastTarget = null;
    }
}
