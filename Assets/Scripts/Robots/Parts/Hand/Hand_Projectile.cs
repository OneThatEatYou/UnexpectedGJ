using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Hand_Projectile : Hand
{
    public Transform bulletSpawnPos;
    public GameObject bulletPrefab;

    public float aimSpeed = 50f;
    public float maxSpeed = 20f;
    public float shootWait = 0.5f;

    public AudioClip shootSFX;

    public override void Action()
    {
        base.Action();

        //aim
        StartCoroutine(AimTowardsPlayer());
    }

    public override void Update()
    {
        base.Update();

        Debug.DrawLine(Controller.PlayerPos.position, transform.position);
    }

    IEnumerator AimTowardsPlayer()
    {
        Debug.Log(gameObject.name + " is aiming");

        Vector2 dir = Controller.PlayerPos.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        Debug.Log($"target angle: {targetAngle}");
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

        Debug.Log("start rotating");

        Quaternion startQuaternion = transform.rotation;
        float angle = transform.localEulerAngles.z;
        float aVel = 0;

        while (transform.rotation != targetQuaternion)
        {
            angle = Mathf.SmoothDampAngle(angle, targetAngle, ref aVel, 1/aimSpeed, maxSpeed);
            //transform.rotation = Quaternion.Lerp(startQuaternion, targetQuaternion, prog);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        Debug.Log(gameObject.name + " finished aiming");

        yield return new WaitForSeconds(shootWait);
        Shoot();

        StartCoroutine(StartCooldown(1f));
    }

    void Shoot()
    {
        AudioManager.PlayAudioAtPosition(shootSFX, bulletSpawnPos.position, AudioManager.sfxMixerGroup);

        Instantiate(bulletPrefab, bulletSpawnPos.position, transform.rotation);
    }
}