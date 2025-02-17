using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "TowerDefense/TurretData")]
public class TurretData : ScriptableObject {
    public GameObject turretPrefab;
    public Sprite previewSprite;
    public int cost;
    public float range;
    public float fireRate;
    // Add any additional variables you need.
}