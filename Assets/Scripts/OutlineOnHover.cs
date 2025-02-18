using UnityEngine;

public class OutlineOnHover : MonoBehaviour
{
    public Color hoverOutlineColor = Color.yellow;  // Color when hovering
    public Color clickOutlineColor = Color.red;     // Color when clicked
    public float outlineThickness = 1.5f;           // Outline thickness

    private Material material;
    private bool isHovered = false;
    private bool isClicked = false;

    void Start()
    {
        // Get the SpriteRenderer's Material
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
            return;
        }

        material = sr.material;

        // Check if the object has the correct tag
        if (!CompareTag("Interactable"))
        {
            enabled = false; // Disable this script if the tag doesn't match
        }
    }

    void OnMouseEnter()
    {
        if (material != null)
        {
            material.SetColor("_OutlineColor", hoverOutlineColor);
            material.SetFloat("_OutlineThickness", outlineThickness);
            isHovered = true;
        }
    }

    void OnMouseExit()
    {
        if (material != null)
        {
            material.SetFloat("_OutlineThickness", 0f); // Remove outline
            isHovered = false;
            isClicked = false;
        }
    }

    void OnMouseDown()
    {
        if (material != null)
        {
            material.SetColor("_OutlineColor", clickOutlineColor);
            isClicked = true;
        }
    }

    void OnMouseUp()
    {
        if (isHovered && material != null)
        {
            material.SetColor("_OutlineColor", hoverOutlineColor);
        }
        else if (material != null)
        {
            material.SetFloat("_OutlineThickness", 0f); // Remove outline
        }

        isClicked = false;
    }
}