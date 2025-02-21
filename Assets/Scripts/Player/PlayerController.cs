using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public Transform aimIndicator;

    [Header("Shooting Settings")]
    public Transform firePoint;
    public BulletPool bulletPool;
    public float fireRate = 0.2f;
    private float nextFireTime = 0f;

    [Header("Dash Settings")]
    public KeyCode dashKey = KeyCode.LeftShift;
    public float dashForce = 515f;
    public float dashDuration = 0.2f;   
    public float dashRechargeTime = 1f; 

    private bool isDashing = false;
    private bool canDash = true;

    private Rigidbody2D rb;
    private Vector2 movement;

    // ADD THIS: reference the Animator
    [Header("Animation")]
    public Animator animator;  // Make sure to assign this in the Inspector.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Ensure Body Type=Dynamic, freeze rotation if desired
    }

    void Update()
    {
        if (!isDashing)
        {
            HandleMovement();
        }
        HandleAiming();
        HandleShooting();
        HandleDashInput();

        // ADD THIS: Update Animator parameters each frame
        UpdateAnimatorParameters();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            // Only apply normal velocity if not dashing
            rb.velocity = movement * moveSpeed;
        }
        // If dashing, we let the AddForce from PerformDash do the work
    }

    // =============== MOVEMENT ===============
    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized;
    }

    // =============== AIM & SHOOT ===============
    void HandleAiming()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        aimIndicator.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = bulletPool.GetPooledObject();
        if (bullet)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = aimIndicator.rotation;
            bullet.SetActive(true);
        }
    }

    // =============== DASH ===============
    void HandleDashInput()
    {
        // If dash key pressed, and we can dash
        if (Input.GetKeyDown(dashKey) && canDash && !isDashing)
        {
            PerformDash();
        }
    }

    void PerformDash()
    {
        isDashing = true;
        canDash = false;  // If you want a brief gap

        // Clear any old velocity so the dash is always consistent
        rb.velocity = Vector2.zero;

        // If you want dash in aim direction, do e.g.:
        // Vector2 dashDir = aimIndicator.right.normalized;
        // Otherwise, dash in movement direction:
        Vector2 dashDir = (movement == Vector2.zero)
                          ? Vector2.right // fallback, or do nothing
                          : movement;

        // Apply an instant force
        rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

        // Wait dashDuration, then restore normal movement
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        yield return new WaitForSeconds(dashDuration);

        // End dash
        isDashing = false;
        rb.velocity = Vector2.zero; // optional: reset velocity

        // Then wait some time if you want a forced delay before next dash
        yield return new WaitForSeconds(dashRechargeTime);
        canDash = true;
    }

    // =============== ANIMATOR UPDATES ===============
    void UpdateAnimatorParameters() // ADD THIS
    {
        // We assume XInput, YInput, Speed exist in your Animator
        animator.SetFloat("XInput", movement.x);
        animator.SetFloat("YInput", movement.y);
        // Use magnitude or sqrMagnitude for Speed. 
        // If your blend tree is normalized, use magnitude.
        animator.SetFloat("Speed", movement.magnitude);
    }
}
