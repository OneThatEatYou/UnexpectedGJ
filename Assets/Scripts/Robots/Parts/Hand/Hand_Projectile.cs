using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHand", menuName = "Parts/Hand/ProjectileHand", order = 3)]
public class Hand_Projectile : Hand
{
    public Transform bulletSpawnOffset;
    public GameObject bulletPrefab;

    private void Update()
    {
        Action();
    }

    public override void Action()
    {
        base.Action();

        //aim
        transform.Rotate(new Vector3(0, 0, 100f*Time.deltaTime));

        //shoot
    }
}
