using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redleg : Leg
{
    public float moveTime;
    public Vector2 jumpPowerRange;

    public override void Action()
    {
        if (Controller.isMoving)
        { return; }

        base.Action();

        Debug.Log("Jumping");

        Vector2 target;
        target.x = Random.Range(RobotBuilder.Instance.playGroundXRange.x, RobotBuilder.Instance.playGroundXRange.y);
        target.y = Controller.transform.position.y;

        StartCoroutine(Jump(target));
    }

    IEnumerator Jump(Vector2 target)
    {
        Controller.isMoving = true;

        float t = 0;
        Vector2 startPos = Controller.transform.position;
        Vector2 currentPos = startPos;
        float jumpPower = Random.Range(jumpPowerRange.x, jumpPowerRange.y);

        while (t != moveTime)
        {
            currentPos.x = Mathf.Lerp(startPos.x, target.x, t / moveTime);
            currentPos.y = jumpPower * Mathf.Sin((Mathf.PI) * (t / moveTime));
            Controller.transform.position = currentPos;
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, moveTime);

            //Debug.Log(t);

            yield return null;
        }

        Controller.isMoving = false;
    }
}
