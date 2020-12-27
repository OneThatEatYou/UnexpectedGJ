using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBullet : BulletController
{
    public int rebounceNum;

    public override void Start()
    {
        rb.AddForce(dir * moveSpeed, ForceMode2D.Impulse);
    }

    public override void OnCollision(Collider2D collision)
    {
        PlayerController player;
        if (collision.TryGetComponent(out player))
        {
            player.Die();
            base.OnCollision(collision);
            Destroy(gameObject);
        }
        else if (rebounceNum > 0)
        {
            rebounceNum--;
            //play squish sfx
        }
        else
        {
            base.OnCollision(collision);
            Destroy(gameObject);
        }
    }

    public override void Move()
    {
        
    }
}
