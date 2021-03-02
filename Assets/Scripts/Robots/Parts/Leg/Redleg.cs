using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redleg : Leg
{
    [Header("Part Specific Settings")]
    public float moveTime;
    public float minDistance;
    public float gravityScale = 2f;

    [Header("Animation Settings")]

    public float crouchAmount;
    public float crouchTime;
    public float holdTime;
    public float releaseTime;
    public float impactMagnitude = 0.3f;
    public float impactDuration = 0.2f;
    public float easeBackDuration = 0.5f;

    public override void Action()
    {
        base.Action();

        Vector2 target = GenerateTarget(minDistance);
        StartCoroutine(Jump(target));
    }

    IEnumerator Jump(Vector2 target)
    {
        float t = 0;
        Vector2 startPos = Controller.transform.position;
        Vector2 currentPos = startPos;

        float jumpPower = -0.5f * (Physics2D.gravity.y * gravityScale) * moveTime;

        Controller.body.PlayCrouchSeq(crouchAmount, crouchTime, holdTime, releaseTime);

        yield return new WaitForSeconds(crouchTime + holdTime + releaseTime / 4);

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

        Controller.body.PlayImpactSeq(Vector2.down * impactMagnitude, impactMagnitude, easeBackDuration);

        CameraController.GenerateImpulse(Vector2.down, 5, 5, 0, 0.3f, 0.5f);
        GenerateCooldown(cooldownRange);
    }
}
