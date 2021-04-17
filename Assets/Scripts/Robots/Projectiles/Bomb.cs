using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BulletController
{
    public override void Start()
    {
        base.Start();

        int randInt = Random.Range(0, 2);

        if (randInt == 0)
        {
            dir.x = -1;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            dir.x = 1;
        }

        dir.y = 0;
    }

    public override void OnCollision(Collider2D collision)
    {
        PlayerController playerController;

        base.OnCollision(collision);

        if (collision.TryGetComponent(out playerController))
        {
            playerController.Die();
        }

        Destroy(gameObject);
    }

    public override void Move()
    {
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
    }
}
