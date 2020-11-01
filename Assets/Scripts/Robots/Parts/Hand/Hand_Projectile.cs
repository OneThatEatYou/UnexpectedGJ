using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand_Projectile : Hand
{
    public Transform bulletSpawnPos;
    public GameObject bulletPrefab;

    public float aimTime = 3f;
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
        //calculate the angle needed for Vector2.left to point at dir.
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        targetAngle += angleOffset;
        targetAngle = Mathf.Repeat(targetAngle, 360);
        float startAngle = transform.eulerAngles.z;
        float currentAngle = startAngle;
        float angleDif = Mathf.Abs(Mathf.DeltaAngle(startAngle, targetAngle));
        float t = 0;
        float localAimTime = aimTime * (angleDif / 360);
        //Debug.Log($"t: {t}, angleDif: {angleDif},startAngle: {startAngle}, targetAngle: {targetAngle}, localAimTime: {localAimTime}");

        //play looping sfx

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) >= 0.01f)
        {
            currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t / localAimTime);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            t += Time.deltaTime;
            //Debug.Log(currentAngle);
            //Debug.Log(Mathf.DeltaAngle(currentAngle, targetAngle));
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
