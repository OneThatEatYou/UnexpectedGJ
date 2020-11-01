﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHead : Head
{
    public Vector2[] laserSpawnPos;
    public GameObject laserPrefab;
    public GameObject chargeParticles;
    public float chargeTime;
    public float aimAngleOffset = 90f;
    public AudioClip shootSFX;

    public override void Action()
    {
        base.Action();

        StartCoroutine(ChargeAndShootLaser());
    }

    IEnumerator ChargeAndShootLaser()
    {
        foreach (Vector2 pos in laserSpawnPos)
        {
            GameObject obj = Instantiate(chargeParticles, (Vector2)transform.position + pos, Quaternion.identity);
            obj.transform.SetParent(gameObject.transform);
        }

        //Debug.Log(name + " is charging");

        yield return new WaitForSeconds(chargeTime);

        AudioManager.PlayAudioAtPosition(shootSFX, laserSpawnPos[0], AudioManager.sfxMixerGroup);

        foreach (Vector2 pos in laserSpawnPos)
        {
            Vector2 dir = (Vector2)Controller.PlayerPos.transform.position - ((Vector2)transform.position + pos);
            float targetAngle = Mathf.Atan2(dir.y, dir.x);
            targetAngle *= Mathf.Rad2Deg;
            Instantiate(laserPrefab, (Vector2)transform.position + pos, Quaternion.Euler(0, 0, targetAngle + aimAngleOffset));
            //Debug.Log(targetAngle);
        }

        StartCoroutine(StartCooldown(1f));
        //Debug.Log(name + " shot a laser beam");
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        for (int i = 0; i < laserSpawnPos.Length; i++)
        {
            Gizmos.DrawWireSphere(laserSpawnPos[i], 0.2f);
        }
    }
}
