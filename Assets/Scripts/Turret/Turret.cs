using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, IDamageable
{
    [Header("Turret Settings")]
    public float health = 100;
    public GameObject rewardPrefab;
    public float fireRate = 1f;
    public float detectionRange = 10f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public int bulletPoolSize = 10;
    public string enemyTag = "Damageable"; // The tag for enemy objects

    [Header("Reload Settings")]
    public int maxBulletsPerReload = 5; // How many bullets per reload
    public float reloadTime = 2f; // Time required to reload
    private int currentBullets; // Tracks remaining bullets before reloading
    private bool isReloading = false;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f; // How fast the turret rotates
    public float rotationThreshold = 5f; // Angle (in degrees) within which the turret is considered "aimed" at the target

    private List<GameObject> bulletPool;
    private float nextFireTime;
    private Transform target; // Enemy target

    void Start()
    {
        InitializeBulletPool();
        currentBullets = maxBulletsPerReload;
    }

    void Update()
    {
        if (isReloading)
            return;

        FindTarget();
        if (target != null)
        {
            // Rotate turret smoothly toward the target.
            SmoothAimAtTarget();

            // Check if turret is aimed at the target within the rotation threshold.
            float currentAngle = transform.eulerAngles.z;
            Vector2 direction = target.position - transform.position;
            float desiredAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, desiredAngle));

            if (angleDifference <= rotationThreshold)
            {
                if (Time.time >= nextFireTime && currentBullets > 0)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                    currentBullets--;

                    if (currentBullets <= 0)
                    {
                        StartCoroutine(Reload());
                    }
                }
            }
        }
    }

    // IDamageable implementation.
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
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

    private void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = detectionRange;
        Transform nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.transform;
            }
        }

        target = nearestEnemy;
    }

    private void SmoothAimAtTarget()
    {
        if (target == null) return;

        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        if (target == null) return;

        GameObject bullet = GetPooledBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            // Set bullet rotation to the firePoint's rotation so it travels straight.
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            // Note: Removed the SetTarget call to ensure the bullet travels in a straight line.
        }
    }

    private void InitializeBulletPool()
    {
        bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
        }
    }

    private GameObject GetPooledBullet()
    {
        foreach (GameObject bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        return null;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentBullets = maxBulletsPerReload;
        isReloading = false;
        Debug.Log("Reload Complete!");
    }
}
