﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenleg : Leg
{
    [Header("Part Specific Settings")]
    [Tooltip("Time taken to move 1 unit")]
    public Vector2 moveSpeedRange;
    public float minDistance = 3f;
    public LayerMask playerLayer;
    public AudioClip strollSFX;

    [Header("Animation settings")]
    //public float angle;
    //public float variation;
    //public float startDuration = 0.5f;
    //public float halfPeriod = 0.1f;
    public string directionParam = "Dir";
    public string strollSpeedParam = "Speed";

    public override void Action()
    {
        base.Action();


        //Debug.Log("Moving");

        Vector2 target = GenerateTarget(minDistance);

        StartCoroutine(Stroll(target));
    }

    IEnumerator Stroll(Vector2 target)
    {
        float t = 0;
        Vector2 startPos = Controller.transform.position;
        Vector2 currentPos = startPos;
        float distance = (startPos - target).magnitude;
        float localMoveTime = distance * Random.Range(moveSpeedRange.x, moveSpeedRange.y);

        Vector2 dir = (target - startPos).normalized;
        ShakeScreen(dir, localMoveTime);

        anim.SetFloat(strollSpeedParam, 1);
        anim.SetFloat(directionParam, dir.x);

        //Debug.Log($"Distance: {distance}, LocalMoveTime: {localMoveTime}");
        AudioSource source = AudioManager.PlayAudioAtPosition(strollSFX, transform.position, AudioManager.sfxMixerGroup, false);
        source.loop = true;

        while (t != localMoveTime)
        {
            currentPos.x = Mathf.SmoothStep(startPos.x, target.x, t / localMoveTime);
            Controller.transform.position = currentPos;
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, localMoveTime);

            yield return null;
        }

        anim.SetFloat(strollSpeedParam, 0);
        Destroy(source.gameObject);
        GenerateCooldown(cooldownRange);
    }

    void ShakeScreen(Vector2 dir, float localMoveTime)
    {
        CameraController.GenerateImpulse(dir, 1f, 4, localMoveTime * 0.1f, localMoveTime * 0.7f, localMoveTime * 1f);
    }
}
