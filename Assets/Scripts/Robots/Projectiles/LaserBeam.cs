using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : BulletController
{
    public override void OnCollision(Collider2D collision)
    {
        base.OnCollision(collision);

        PlayerController player;
        if (collision.TryGetComponent(out player))
        {
            player.Die();
        }

        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }
}
