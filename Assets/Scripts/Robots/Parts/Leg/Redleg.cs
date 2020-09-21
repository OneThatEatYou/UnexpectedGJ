using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redleg : Leg
{
    public float moveTime;
    public float minDistance;
    public float gravityScale = 2f;

    public override void Action()
    {
        if (Controller.isMoving)
        { return; }

        base.Action();

        //Debug.Log("Jumping");

        Vector2 target = GenerateTarget(minDistance, 10);

        StartCoroutine(Jump(target));
    }

    IEnumerator Jump(Vector2 target)
    {
        Controller.isMoving = true;

        float t = 0;
        Vector2 startPos = Controller.transform.position;
        Vector2 currentPos = startPos;

        float jumpPower = -0.5f * (Physics2D.gravity.y * gravityScale) * moveTime;

        while (t != moveTime)
        {
            currentPos.x = Mathf.Lerp(startPos.x, target.x, t / moveTime);
            currentPos.y = jumpPower * t + 0.5f * (Physics2D.gravity.y * gravityScale) * t * t;
            Controller.transform.position = currentPos;
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, moveTime);

            //Debug.Log(t);

            yield return null;
        }

        CameraController.GenerateImpulse(Vector2.down, 5, 5, 0, 0.3f, 0.5f);
        Controller.isMoving = false;
    }
}
