using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 10f; // Set lifetime to 10 seconds
    public int damage = 10;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rb.velocity = transform.right * speed; // Use velocity for movement
        Invoke(nameof(DisableBullet), lifeTime); // Disable bullet after 10 seconds
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Bullet Hit", transform.position);
            damageable.TakeDamage(damage);
        }

        DisableBullet(); // Disable bullet after hitting something
    }

    void DisableBullet()
    {
        CancelInvoke(nameof(DisableBullet)); // Cancel any existing disable invoke
        gameObject.SetActive(false); // Return bullet to pool
    }
}