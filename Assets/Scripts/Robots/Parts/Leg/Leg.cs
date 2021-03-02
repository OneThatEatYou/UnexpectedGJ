using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : BasePart
{
    public override void Start()
    {
        base.Start();

        Controller.AddNonDetachablePart(this);
    }

    public override void Action()
    {
        base.Action();

        Controller.canMove = false;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Controller.TakeDamage(maxHealth);
        }
    }

    public override void Detach()
    {
        base.Detach();

        Explode();
    }

    //completely overwrite cooldown for leg. set Controller.canMove to true instead of isReady
    public override void GenerateCooldown(Vector2 cdRange)
    {
        float cd = Random.Range(cdRange.x, cdRange.y);
        if (onGenerateCooldown != null)
        {
            cd = onGenerateCooldown(cd);
        }

        StartCoroutine(ReadyLegAfterCooldown(cd));
    }

    //let the robotController know that it can move after cooldown
    public IEnumerator ReadyLegAfterCooldown(float cd)
    {
        yield return new WaitForSeconds(cd);

        Controller.canMove = true;
    }

    public virtual Vector2 GenerateTarget(float minDistance)
    {
        //stores value to be returned
        Vector2 target;

        //stores sets of points to choose from: left and right
        List<Vector2> ranges = new List<Vector2>();
        int rangeInt = 0;
        float minPos = Controller.transform.position.x - minDistance;
        float maxPos = Controller.transform.position.x + minDistance;

        if (minPos > RobotBuilder.Instance.playGroundXRange.x)
        {
            //can generate point from left set
            Vector2 setL;
            setL.x = RobotBuilder.Instance.playGroundXRange.x;
            setL.y = minPos;
            ranges.Add(setL);
        }
        if (maxPos < RobotBuilder.Instance.playGroundXRange.y)
        {
            //can generate point from right set
            Vector2 setR;
            setR.x = maxPos;
            setR.y = RobotBuilder.Instance.playGroundXRange.y;
            ranges.Add(setR);
        }

        if (ranges.Count == 0)
        {
            Debug.LogError($"No suitable range of points found for {name}. minPos: {minPos}, maxPos: {maxPos}");
            return Vector2.zero;
        }
        else if (ranges.Count > 1)
        {
            //choose random range
            rangeInt = Random.Range(0, ranges.Count);
        }

        //choose a point from choosen range
        target.x = Random.Range(ranges[rangeInt].x, ranges[rangeInt].y);
        target.y = Controller.transform.position.y;

        return target;
    }
}
