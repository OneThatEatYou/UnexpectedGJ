using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBullet : BulletController
{
    public float explosionRad;
    public float maxAngleDelta = 0.5f;
    public float accel;
    public float followTime = 3f;
    public float animationStartTime;
    [Header("Steppable Area")]
    public Vector2 offset;
    public Vector2 size;

    float elapsedTime;
    bool isActivated;

    public override void OnCollision(Collider2D collision)
    {
        Collider2D col = Physics2D.OverlapCircle(rb.position, explosionRad, effectLayer);

        PlayerController player;
        if (col && col.TryGetComponent(out player))
        {
            player.Die();
        }

        base.OnCollision(collision);

        Destroy(gameObject);
    }

    public override void Move()
    {
        elapsedTime += Time.fixedDeltaTime;
        float v = Mathf.Clamp(accel * elapsedTime, 0, moveSpeed);
        base.Move();

        if (elapsedTime < followTime && target)
        {
            dir = target.position - transform.position;
            Quaternion targetRotation = Quaternion.FromToRotation(-Vector2.up, dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, maxAngleDelta * Time.fixedDeltaTime * (1 - elapsedTime / followTime));
        }

        if (elapsedTime > animationStartTime && !isActivated && anim)
        {
            anim.SetBool("isActive", true);
            isActivated = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRad);
    }
}
