using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand_Pyro : Hand
{
    //public Vector2 attackOffset;
    //public Vector2 AttackPos { get { return (Vector2)transform.position + attackOffset; } }
    public Transform attackPos;
    public Vector2 attackSize;
    public LayerMask effectLayer;

    public float aimSpeed;
    public float shootWait;
    public float fireDur;

    bool isAttacking = false;
    ParticleSystem fireParticle;
    AudioSource aSource;

    public override void Awake()
    {
        base.Awake();

        fireParticle = GetComponentInChildren<ParticleSystem>();
        aSource = GetComponent<AudioSource>();
    }

    public override void Action()
    {
        base.Action();

        StartCoroutine(AimTowardsPlayer());
    }

    IEnumerator AimTowardsPlayer()
    {
        Vector2 dir;
        if (Controller.PlayerPos)
        {
            dir = Controller.PlayerPos.transform.position - transform.position;
        }
        else
        {
            dir = Controller.GenerateRandomPosition() - (Vector2)transform.position;
        }
        dir.Normalize();
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, targetAngle);

        //AudioManager.PlayAudioAtPosition(rotationSFX, transform.position, AudioManager.sfxMixerGroup);

        Sequence seq = DOTween.Sequence();

        //time needed to rotate towards player
        float t = Quaternion.Angle(transform.rotation, targetQuaternion) / 180 * aimSpeed;
        seq.Append(transform.DOLocalRotateQuaternion(targetQuaternion, t).SetEase(Ease.OutCubic));
        seq.AppendInterval(shootWait);

        seq.AppendCallback(ToggleFire);
        float panAngle = Random.Range(0, 2) == 0 ? -120 : 120;
        targetQuaternion = Quaternion.Euler(0, 0, targetAngle + panAngle);
        seq.Append(transform.DOLocalRotateQuaternion(targetQuaternion, fireDur));
        seq.AppendCallback(ToggleFire);

        yield return new WaitForSeconds(seq.Duration());

        GenerateCooldown(cooldownRange);
    }

    void ToggleFire()
    {
        if (!isAttacking)
        {
            fireParticle.Play();
            InvokeRepeating("Attack", 0, 0.1f);
            aSource.Play();
        }
        else
        {
            fireParticle.Stop();
            CancelInvoke("Attack");
            aSource.Stop();
        }

        isAttacking = !isAttacking;
    }

    void Attack()
    {
        PlayerController player;

        Collider2D[] cols = Physics2D.OverlapBoxAll(attackPos.position, attackSize, transform.eulerAngles.z, effectLayer);

        foreach (var col in cols)
        {
            Debug.Log($"Collided with {col.name}");
            if (col.TryGetComponent(out player))
            {
                player.Die();
            }
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;

        if (attackPos)
        {
            Gizmos.DrawWireCube(attackPos.position, attackSize);
        }
    }
}
