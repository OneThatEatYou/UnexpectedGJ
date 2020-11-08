using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (Physics2D.OverlapCircle(transform.position, unreachableRad, targetLayer))
        {
            StartCoroutine(StartCooldown(0.5f));
            return;
        }

        //aim
        StartCoroutine(AimTowardsPlayer());
    }

    IEnumerator AimTowardsPlayer()
    {
        //Debug.Log(gameObject.name + " is aiming");

        Vector2 dir = Controller.PlayerPos.transform.position - transform.position;
        Quaternion targetQuaternion = Quaternion.FromToRotation(Vector2.down, dir) * Quaternion.Euler(0, 0, angleOffset);

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
