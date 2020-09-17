using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenleg : Leg
{
    [Tooltip("Time taken to move 1 unit")]
    public Vector2 moveTimeRange;
    public int crushDamage = 1;
    public Vector2 crushBoxOffset;
    public Vector2 crushBoxSize;
    public LayerMask playerLayer;

    public override void Update()
    {
        base.Update();

        Collider2D col = Physics2D.OverlapBox((Vector2)transform.position + crushBoxOffset, crushBoxSize, 0, playerLayer);
        PlayerController playerCon;

        if (col)
        {
            if (col.TryGetComponent(out playerCon))
            {
                if (col.GetComponent<Rigidbody2D>().velocity.y < 0)
                {
                    //only deal crush damage when player is moving downwards
                    playerCon.TakeDamage(crushDamage);
                }
            }
        }
    }

    public override void Action()
    {
        if (Controller.isMoving)
        { return; }

        base.Action();

        //Debug.Log("Moving");

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
        float distance = (startPos - target).magnitude;
        float localMoveTime = distance * Random.Range(moveTimeRange.x, moveTimeRange.y);

        //Debug.Log($"Distance: {distance}, LocalMoveTime: {localMoveTime}");

        while (t != localMoveTime)
        {
            currentPos.x = Mathf.Lerp(startPos.x, target.x, t / localMoveTime);
            Controller.transform.position = currentPos;
            t += Time.deltaTime;
            t = Mathf.Clamp(t, 0, localMoveTime);

            //Debug.Log(t);

            yield return null;
        }

        Controller.isMoving = false;
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + crushBoxOffset, crushBoxSize);
    }
}
