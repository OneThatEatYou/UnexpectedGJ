using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : BasePart
{
    public override void Start()
    {
        base.Start();

        Controller.AddNonDetachablePart(this);
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
}
