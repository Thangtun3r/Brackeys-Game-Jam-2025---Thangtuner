using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    public float healthPoint = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IHealable healable = other.GetComponent<IHealable>();
        if (healable != null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heath", transform.position);
            healable.Heal(healthPoint);
            Destroy(gameObject);
        }
    }
}
