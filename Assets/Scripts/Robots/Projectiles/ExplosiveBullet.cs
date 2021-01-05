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
    [Header("Particle")]
    public ParticleSystem exhaustPS;

    float elapsedTime;
    bool isActivated;

    public override void OnCollision(Collider2D collision)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(rb.position, explosionRad, effectLayer);

        PlayerController player;
        foreach (var col in colls)
        {
            //check if player
            if (col.TryGetComponent(out player))
            {
                player.Die();
                break;
            }
        }

        base.OnCollision(collision);

        //destroy smoke particle after its lifetime
        exhaustPS.transform.parent = null;
        exhaustPS.Stop();
        Destroy(exhaustPS.gameObject, exhaustPS.main.startLifetime.constantMax);

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
            exhaustPS.Play();
            isActivated = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRad);
    }
}
