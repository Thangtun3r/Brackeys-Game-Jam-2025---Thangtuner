using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    public int maxHealth = 100;
    public float moveSpeed = 2f;
}