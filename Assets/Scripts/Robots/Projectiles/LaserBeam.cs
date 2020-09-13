using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : BulletController
{
    public override void OnTrigger(Collider2D collision)
    {
        base.OnTrigger(collision);

        PlayerController player;
        if (collision.TryGetComponent(out player))
        {
            player.TakeDamage(1);
        }

        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }
}
