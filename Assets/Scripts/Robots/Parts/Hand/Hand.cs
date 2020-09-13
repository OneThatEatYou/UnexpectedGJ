using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : BasePart
{
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
}
