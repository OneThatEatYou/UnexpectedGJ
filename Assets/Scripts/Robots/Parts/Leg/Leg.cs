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

    public IEnumerator ReadyLegAfterCooldown()
    {
        yield return new WaitForSeconds(Cooldown);

        Controller.canMove = true;
    }

    public virtual Vector2 GenerateTarget(float minDistance, int numOfTries)
    {
        Vector2 target;
        target.x = Random.Range(RobotBuilder.Instance.playGroundXRange.x, RobotBuilder.Instance.playGroundXRange.y);
        target.y = Controller.transform.position.y;

        Vector2 direction = target - (Vector2)transform.position;
        float distance = direction.magnitude;
        int tries = 0;

        //while distance is too little and target outside bounds
        while (distance < minDistance || (target.x < RobotBuilder.Instance.playGroundXRange.x || target.x > RobotBuilder.Instance.playGroundXRange.y))
        {
            if (tries < numOfTries)
            {
                if (distance < minDistance)
                {
                    if (direction.x < 0)
                    {
                        //move more to the left
                        target.x -= minDistance;
                    }
                    else
                    {
                        //move more to the right
                        target.x += minDistance;
                    }

                    //check if out of bounds
                    if (target.x < RobotBuilder.Instance.playGroundXRange.x || target.x > RobotBuilder.Instance.playGroundXRange.y)
                    {
                        Random.Range(RobotBuilder.Instance.playGroundXRange.x, RobotBuilder.Instance.playGroundXRange.y);
                    }

                    direction = target - (Vector2)transform.position;
                    distance = direction.magnitude;

                    if (tries == numOfTries)
                    {
                        Debug.Log("No suitable target found.");
                        target = Vector2.zero;
                    }

                    tries++;
                }
                else
                {
                    Debug.Log("No suitable target found. Last target is " + target);
                    target = Vector2.zero;
                    
                    break;
                }
            }
        }
        return target;
    }
}
