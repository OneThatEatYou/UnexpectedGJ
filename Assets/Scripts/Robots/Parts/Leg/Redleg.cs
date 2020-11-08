﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redleg : Leg
{
    [Header("Part Specific Settings")]
    public float moveTime;
    public float minDistance;
    public float gravityScale = 2f;

    public float crouchAmount;
    public float crouchTime;
    public float holdTime;
    public float releaseTime;

    public override void Action()
    {
        base.Action();

        //Debug.Log("Jumping");

        Vector2 target = GenerateTarget(minDistance);
        //Debug.Log("target:" + target);

        StartCoroutine(Jump(target));
    }

    IEnumerator Jump(Vector2 target)
    {
        float t = 0;
        Vector2 startPos = Controller.transform.position;
        Vector2 currentPos = startPos;

        float jumpPower = -0.5f * (Physics2D.gravity.y * gravityScale) * moveTime;

        for (int i = 0; i < Controller.bodies.Count; i++)
        {
            Controller.bodies[i].Crouch(crouchAmount, crouchTime, holdTime, releaseTime);
        }

        yield return new WaitForSeconds(crouchTime + holdTime + (releaseTime / 2));

        while (t != moveTime)
        {
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, moveTime);
            currentPos.x = Mathf.Lerp(startPos.x, target.x, t / moveTime);
            currentPos.y = jumpPower * t + 0.5f * (Physics2D.gravity.y * gravityScale) * t * t;
            Controller.transform.position = currentPos;
            

            //Debug.Log(t);

            yield return null;
        }

        CameraController.GenerateImpulse(Vector2.down, 5, 5, 0, 0.3f, 0.5f);
        StartCoroutine(ReadyLegAfterCooldown());
    }
}
