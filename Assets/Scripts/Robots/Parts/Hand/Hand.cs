using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : BasePart
{
    [Header("Part Specific Settings")]
    public float angleOffset;
    public float unreachableRad;
    public LayerMask targetLayer;

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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, unreachableRad);
    }
}
