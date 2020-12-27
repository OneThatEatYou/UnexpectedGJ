using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public class Hand_Projectile : Hand
{
    public Transform bulletSpawnPos;
    public GameObject bulletPrefab;
    public bool rotateBullet = true;

    public float aimSpeed = 50f;
    public float maxSpeed = 20f;
    public float shootWait = 0.5f;

    public AudioClip shootSFX;

    [Header("Tweening settings")]
    public AnimationCurve rotationCurve;
    public float expand = 0.03f;
    public float expandRTime = 0.02f;
    public float recoil = 0.8f;
    public float recoilTime = 0.5f;
    public float recoilRTime = 0.2f;

    public override void Action()
    {
        base.Action();

        //aim
        StartCoroutine(AimTowardsPlayer());
    }

    public override void Update()
    {
        base.Update();

        //Debug.DrawLine(Controller.PlayerPos.position, transform.position);
    }

    IEnumerator AimTowardsPlayer()
    {
        //Debug.Log(gameObject.name + " is aiming");

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
        //Debug.Log($"target angle: {targetAngle}");
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, targetAngle);

        //check if player is in unreachable range
        //if currently in red range, can only rotate within red range
        if (Physics2D.OverlapCircle(transform.position, unreachableRad, targetLayer))
        {
            if (Mathf.DeltaAngle(transform.localEulerAngles.z, redAngleEnd.z) < 0 && Mathf.DeltaAngle(transform.localEulerAngles.z, redAngleStart.z) > 0)
            {
                //currently in minor arc
                //Debug.Log($"Player in minor arc. Angle difference: {Mathf.DeltaAngle(transform.localEulerAngles.z, redAngleEnd.z)}");

                if (arcType == RedArcType.Minor)
                {
                    if (Mathf.DeltaAngle(targetAngle, redAngleEnd.z) > 0 || Mathf.DeltaAngle(targetAngle, redAngleStart.z) < 0)
                    {
                        //target is in major arc
                        //abort
                        //Debug.Log("Target is in major arc. Aborting.");
                        StartCoroutine(StartCooldown(0.5f));
                        yield break;
                    }
                }
            }
            else
            {
                //currently in major arc
                //Debug.Log($"Player in major arc. Angle difference: {Mathf.DeltaAngle(transform.localEulerAngles.z, redAngleEnd.z)}");

                if (arcType == RedArcType.Major)
                {
                    if (Mathf.DeltaAngle(targetAngle, redAngleEnd.z) < 0 && Mathf.DeltaAngle(targetAngle, redAngleStart.z) > 0)
                    {
                        //target is in minor arc
                        //abort
                        //Debug.Log("Target is in minor arc. Aborting.");
                        StartCoroutine(StartCooldown(0.5f));
                        yield break;
                    }
                }
            }
        }

        //play looping sfx

        //Debug.Log("start rotating");

        //linear
        float t = Quaternion.Angle(transform.rotation, targetQuaternion) / 180 * aimSpeed;

        transform.DOLocalRotateQuaternion(targetQuaternion, t).SetEase(rotationCurve);
        yield return new WaitForSeconds(t);

        //Debug.Log(gameObject.name + " finished aiming");

        //excrete
        Sequence s1 = DOTween.Sequence();
        s1.Append(transform.DOScale(Vector2.one * expand, shootWait).SetRelative().SetEase(Ease.OutCubic));
        s1.Insert(shootWait, transform.DOScale(Vector2.one, expandRTime).SetEase(Ease.OutBack));

        yield return new WaitForSeconds(shootWait);
        Shoot(dir);

        //recoil
        Sequence s2 = DOTween.Sequence();
        s2.Append(transform.DOLocalMove(transform.up * recoil, recoilTime).SetRelative());
        s2.Append(transform.DOLocalMove(transform.up * -recoil, recoilRTime).SetRelative().SetEase(Ease.InQuad));

        StartCoroutine(StartCooldown(1f));
    }

    void Shoot(Vector2 dir)
    {
        AudioManager.PlayAudioAtPosition(shootSFX, bulletSpawnPos.position, AudioManager.sfxMixerGroup);

        BulletController bul;
        if (rotateBullet)
        {
            bul = Instantiate(bulletPrefab, bulletSpawnPos.position, transform.rotation).GetComponent<BulletController>();
        }
        else
        {
            bul = Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity).GetComponent<BulletController>();
        }
        bul.dir = dir;

        if (Controller.PlayerPos)
        {
            bul.target = Controller.PlayerPos;
        }
    }
}