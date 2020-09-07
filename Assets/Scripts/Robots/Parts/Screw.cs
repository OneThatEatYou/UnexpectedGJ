using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : MonoBehaviour
{
    [HideInInspector] public RobotController controller;

    public int maxHealth;
    int currentHealth;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Unscrew()
    {
        if (currentHealth <= 0)
        { return; }

        Debug.Log($"Unscrewing {name}.");

        //animation

        //movement

        currentHealth--;

        if (currentHealth <= 0)
        {
            Detach();
        }
    }

    private void Detach()
    {
        Debug.Log("Detached a screw");

        rb.bodyType = RigidbodyType2D.Dynamic;

        if (controller)
        {
            controller.TakeDamage(maxHealth);
        }
        else
        {
            Debug.LogWarning($"Controller for {name} not found.");
        }
    }
}
