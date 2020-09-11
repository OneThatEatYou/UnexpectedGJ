using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : MonoBehaviour
{
    public int maxHealth;
    int currentHealth;

    public Vector2 screwStartRange;
    public Vector2 screwEndRange;

    private RobotController controller;
    public RobotController Controller
    {
        get { return controller; }
    }

    public virtual void Awake()
    {
        controller = GetComponentInParent<RobotController>();
    }

    public virtual void Start()
    {
        currentHealth = maxHealth;
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

        //explosion

        //screen shake

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(screwStartRange, screwEndRange);
    }
}
