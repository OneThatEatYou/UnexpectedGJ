using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartController : MonoBehaviour
{
    public BasePart part;
    public RobotController robotController;

    public int maxHealth;
    int currentHealth;

    private void Start()
    {
        maxHealth = part.health;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        part.Action();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Detach();
        }
    }

    public void Detach()
    {
        robotController.TakeDamage(maxHealth);
    }
}
