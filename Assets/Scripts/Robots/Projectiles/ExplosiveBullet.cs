using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : BulletController
{
    public float explosionRad;
    public override void OnCollide()
    {
        Collider2D col = Physics2D.OverlapCircle(rb.position, explosionRad, effectLayer);

        PlayerController player;
        if (col && col.TryGetComponent(out player))
        {
            player.TakeDamage(1);
        }

        base.OnCollide();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRad);
    }
}
