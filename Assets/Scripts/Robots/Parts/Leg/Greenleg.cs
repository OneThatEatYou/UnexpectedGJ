using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenleg : Leg
{
    public float moveTime;

    public override void Action()
    {
        if (Controller.isMoving)
        { return; }

        base.Action();

        Debug.Log("Jumping");

        Vector2 target;
        target.x = Random.Range(RobotBuilder.Instance.playGroundXRange.x, RobotBuilder.Instance.playGroundXRange.y);
        target.y = Controller.transform.position.y;

        StartCoroutine(Stroll(target));
    }

    IEnumerator Stroll(Vector2 target)
    {
        Controller.isMoving = true;

        float t = 0;
        Vector2 startPos = Controller.transform.position;
        Vector2 currentPos = startPos;

        while (t != moveTime)
        {
            currentPos.x = Mathf.Lerp(startPos.x, target.x, t / moveTime);
            Controller.transform.position = currentPos;
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, moveTime);

            //Debug.Log(t);

            yield return null;
        }

        Controller.isMoving = false;
    }
}
