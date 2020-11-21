using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : BulletController
{
    public float explosionRad;
    public float maxAngleDelta = 100;
    public override void OnCollide(Collision2D collision)
    {
        Collider2D col = Physics2D.OverlapCircle(rb.position, explosionRad, effectLayer);

        PlayerController player;
        if (col && col.TryGetComponent(out player))
        {
            player.TakeDamage(1);
        }

        base.OnCollide(collision);
    }

    public override void Move()
    {
        base.Move();

        Vector3 dir = target.position - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(-Vector2.up, dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, maxAngleDelta * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRad);
    }
}
