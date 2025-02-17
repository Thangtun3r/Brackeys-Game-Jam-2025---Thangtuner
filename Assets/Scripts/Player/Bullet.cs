using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rb.velocity = transform.right * speed; // Use velocity instead of Translate
        Invoke(nameof(DisableBullet), lifeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(10f);
        }

        gameObject.SetActive(false); // Return to pool on any collision
    }

    void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}