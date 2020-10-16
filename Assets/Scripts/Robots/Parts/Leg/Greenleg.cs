using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenleg : Leg
{
    [Tooltip("Time taken to move 1 unit")]
    public Vector2 moveTimeRange;
    public float minDistance = 3f;
    public int crushDamage = 1;
    public Vector2 crushBoxOffset;
    public Vector2 crushBoxSize;
    public LayerMask playerLayer;

    public override void Action()
    {
        if (Controller.isMoving)
        { return; }

        base.Action();

        //Debug.Log("Moving");

        Vector2 target = GenerateTarget(minDistance, 10);

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

        ShakeScreen(localMoveTime);

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

    void ShakeScreen(float localMoveTime)
    {
        CameraController.GenerateImpulse(0.5f, 4, localMoveTime * 0.1f, localMoveTime * 0.7f, localMoveTime * 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerCon;
        if (collision.TryGetComponent(out playerCon))
        {
            playerCon.TakeDamage(crushDamage);
            Debug.Log("Player took crush damage");
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + crushBoxOffset, crushBoxSize);
    }
}
