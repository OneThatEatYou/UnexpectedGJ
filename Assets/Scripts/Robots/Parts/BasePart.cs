using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : MonoBehaviour
{
    public int maxHealth;
    int currentHealth;

    private RobotController controller;
    public RobotController Controller
    {
        get { return controller; }
        set { controller = value; }
    }

    public virtual void Action()
    {
        
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Detach();
        }
    }

    public virtual void Detach()
    {
        controller.TakeDamage(maxHealth);
    }
}
