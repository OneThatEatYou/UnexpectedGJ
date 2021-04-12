using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pyroleg : Leg
{
    [Header("Movement")]
    public float minDistance = 1;
    public float raiseTime = 1; // time taken to move to raiseHeight
    public float raiseHeight;   // how high the robot is to be before moving
    public float moveSpeed = 1;

    [Header("Attack")]
    public Vector2 attackPosition;
    public Vector2 attackSize;
    public LayerMask effectLayer;

    bool hiboxIsActive = false;

    public override void Update()
    {
        base.Update();

        if (hiboxIsActive)
        {
            Attack();
        }
    }

    public override void Action()
    {
        base.Action();

        Vector2 target = GenerateTarget(minDistance);
        StartCoroutine(Move(target));
    }

    IEnumerator Move(Vector2 target)
    {
        Debug.Log("Start moving");
        Sequence seq = DOTween.Sequence();

        //move up
        seq.Append(Controller.transform.DOMoveY(raiseHeight, raiseTime).SetRelative());
        seq.AppendCallback(ToggleAttack);
        //move horizontal
        float moveTime = (target.x - Controller.transform.position.x) / moveSpeed;
        seq.Append(Controller.transform.DOMoveX(target.x, moveTime).SetRelative());
        seq.AppendCallback(ToggleAttack);
        //move down
        seq.Append(Controller.transform.DOMoveY(-raiseHeight, raiseTime).SetRelative());

        //activate particle

        yield return new WaitForSeconds(raiseTime * 2 + moveTime);

        GenerateCooldown(cooldownRange);
        Debug.Log("Finished moving");
    }

    void ToggleAttack()
    {
        hiboxIsActive = !hiboxIsActive;
    }

    void Attack()
    {
        PlayerController player;

        Collider2D[] cols = Physics2D.OverlapBoxAll(attackPosition, attackSize, 0, effectLayer);

        foreach (var col in cols)
        {
            if (col.TryGetComponent(out player))
            {
                player.Die();
            }
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.red;
        Gizmos.DrawCube(attackPosition, attackSize);
    }
}
