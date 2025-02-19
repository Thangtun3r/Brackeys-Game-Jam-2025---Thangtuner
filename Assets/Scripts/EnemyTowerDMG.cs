using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTowerDMG : MonoBehaviour
{
    public float damage = 10;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("hit");
            damageable.TakeDamage(damage);
        }
    }
}
