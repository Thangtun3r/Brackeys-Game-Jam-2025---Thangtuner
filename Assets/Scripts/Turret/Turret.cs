using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour, IDamageable
{
    public TurretData turretData;

    [Header("Turret Settings")]
    public int health = 100;
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
            SmoothAimAtTarget(); // Now uses Lerp for smooth rotation
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

    // Implementing IDamageable
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " destroyed!");
        gameObject.SetActive(false); // Simulate destruction
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
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            bullet.GetComponent<TurretBullet>().SetTarget(target);
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
