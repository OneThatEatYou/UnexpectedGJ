using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Hand_Projectile : Hand
{
    public Transform bulletSpawnPos;
    public GameObject bulletPrefab;

    public float aimSpeed = 50f;
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
        Quaternion targetQuaternion = Quaternion.FromToRotation(Vector2.down, dir) * Quaternion.Euler(0, 0, angleOffset);
        Quaternion redAngleStartQuaternion = Quaternion.Euler(redAngleStart.x, redAngleStart.y, redAngleStart.z);
        Quaternion redAngleEndQuaternion = Quaternion.Euler(redAngleEnd.x, redAngleEnd.y, redAngleEnd.z);

        //check if player is in unreachable range
        //if currently in red range, can only rotate within red range
        if (Physics2D.OverlapCircle(transform.position, unreachableRad, targetLayer))
        {
            Debug.Log("Player is in unreachable radius");

            //Quaternion.Angle always returns positive value

            if (Quaternion.Angle(transform.localRotation, redAngleEndQuaternion) < 0 && Quaternion.Angle(transform.localRotation, redAngleStartQuaternion) > 0)
            {
                //currently in minor arc
                Debug.Log($"Player in minor arc. Angle difference: {Quaternion.Angle(transform.localRotation, redAngleEndQuaternion)}");

                if (arcType == RedArcType.Minor)
                {
                    if (Quaternion.Angle(targetQuaternion, redAngleEndQuaternion) > 0 || Quaternion.Angle(targetQuaternion, redAngleStartQuaternion) < 0)
                    {
                        //target is in major arc
                        //abort
                        Debug.Log("Target is in major arc. Aborting.");
                        StartCoroutine(StartCooldown(0.5f));
                        yield break;
                    }
                }
            }
            else
            {
                //currently in major arc
                Debug.Log($"Player in major arc. Angle difference: {Quaternion.Angle(transform.localRotation, redAngleEndQuaternion)}");

                if (arcType == RedArcType.Major)
                {
                    if (Quaternion.Angle(targetQuaternion, redAngleEndQuaternion) < 0 && Quaternion.Angle(targetQuaternion, redAngleStartQuaternion) > 0)
                    {
                        //target is in minor arc
                        //abort
                        Debug.Log("Target is in minor arc. Aborting.");
                        StartCoroutine(StartCooldown(0.5f));
                        yield break;
                    }
                }
            }
        }

        //play looping sfx

        while (transform.rotation != targetQuaternion)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, aimSpeed * Time.deltaTime);

            yield return null;
        }

        //Debug.Log(gameObject.name + " finished aiming");

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