using UnityEngine;

public class FlipSpriteByCursor : MonoBehaviour
{
    public Transform referenceObject; // Assign the player or gun holder object in the inspector
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (referenceObject == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ignore the z-axis for 2D games
        bool facingRight = mousePosition.x > referenceObject.position.x;

        // Flip sprite
        spriteRenderer.flipX = !facingRight;

        // Adjust gun rotation
        float angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg;

        if (!facingRight)
        {
            angle += 180f; // Fix rotation when flipped
        }

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}