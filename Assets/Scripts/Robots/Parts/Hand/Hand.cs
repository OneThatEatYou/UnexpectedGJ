using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Hand : BasePart
{
    [Header("Part Specific Settings")]
    public float angleOffset;
    public float unreachableRad;
    public LayerMask targetLayer;

    public Vector3 redAngleStart;
    public Vector3 redAngleEnd;
    public RedArcType arcType = RedArcType.Minor;
    public enum RedArcType
    {
        Minor,
        Major
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Controller.TakeDamage(maxHealth);
            Detach();
        }
    }

    public override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }
}

[CustomEditor(typeof(Hand_Projectile))]
public class DrawWireArc : Editor
{
    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        Hand myObj = (Hand)target;
        Vector2 dir = new Vector2(Mathf.Cos(myObj.redAngleEnd.z * Mathf.Deg2Rad), Mathf.Sin(myObj.redAngleEnd.z * Mathf.Deg2Rad));
        Vector3 angleDiff;
        if (myObj.arcType == Hand.RedArcType.Minor)
        {
            angleDiff = Vector3.forward * Mathf.DeltaAngle(myObj.redAngleEnd.z, myObj.redAngleStart.z);
            Handles.DrawWireArc(myObj.transform.position, Vector3.forward, dir, angleDiff.z, myObj.unreachableRad);
            Handles.DrawLine(myObj.transform.position, (Vector2)myObj.transform.position + (dir * myObj.unreachableRad));
            Handles.DrawLine(myObj.transform.position, GameManager.RotatePointAroundPivot((Vector2)myObj.transform.position + (dir * myObj.unreachableRad), myObj.transform.position, angleDiff));
            Handles.color = Color.green;
            Handles.DrawWireArc(myObj.transform.position, Vector3.forward, dir, angleDiff.z - 360, myObj.unreachableRad);
        }
        else
        {
            angleDiff = Vector3.forward * Mathf.DeltaAngle(myObj.redAngleEnd.z, myObj.redAngleStart.z);
            angleDiff.z -= 360;
            Handles.DrawWireArc(myObj.transform.position, Vector3.forward, dir, angleDiff.z, myObj.unreachableRad);
            Handles.DrawLine(myObj.transform.position, (Vector2)myObj.transform.position + (dir * myObj.unreachableRad));
            Handles.DrawLine(myObj.transform.position, GameManager.RotatePointAroundPivot((Vector2)myObj.transform.position + (dir * myObj.unreachableRad), myObj.transform.position, angleDiff));
            Handles.color = Color.green;
            Handles.DrawWireArc(myObj.transform.position, Vector3.forward, dir, angleDiff.z + 360, myObj.unreachableRad);
        }
    }
}
