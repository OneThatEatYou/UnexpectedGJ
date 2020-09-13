using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHead : Head
{
    public Vector2[] laserSpawnPos;
    public GameObject laserPrefab;
    public GameObject chargeParticles;
    public float chargeTime;
    bool isCharging = false;
    public float aimAngleOffset = 90f;

    public override void Action()
    {
        if (isCharging)
        { return; }

        base.Action();

        StartCoroutine(ChargeAndShootLaser());
    }

    IEnumerator ChargeAndShootLaser()
    {
        isCharging = true;

        foreach (Vector2 pos in laserSpawnPos)
        {
            Instantiate(chargeParticles, (Vector2)transform.position + pos, Quaternion.identity);
        }

        //Debug.Log(name + " is charging");

        yield return new WaitForSeconds(chargeTime);

        foreach (Vector2 pos in laserSpawnPos)
        {
            Vector2 dir = (Vector2)Controller.PlayerPos.transform.position - ((Vector2)transform.position + pos);
            float targetAngle = Mathf.Atan2(dir.y, dir.x);
            targetAngle *= Mathf.Rad2Deg;
            Instantiate(laserPrefab, (Vector2)transform.position + pos, Quaternion.Euler(0, 0, targetAngle + aimAngleOffset));
            //Debug.Log(targetAngle);
        }

        isCharging = false;
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
