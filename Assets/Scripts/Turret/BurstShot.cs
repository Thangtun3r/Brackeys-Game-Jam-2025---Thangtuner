using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SingleShot : IShootingStyle
{
    public void Shoot(GameObject bullet)
    {
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.forward * 10f;
    }
}
