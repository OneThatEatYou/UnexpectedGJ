﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// handles when modules are executed
/// </summary>
public class RobotController : MonoBehaviour
{
    Transform playerPos;
    public Transform PlayerPos { get { return playerPos; } }

    public List<BasePart> parts = new List<BasePart>();
    public List<Body> bodies = new List<Body>();
    public List<Hand> hands = new List<Hand>();
    public List<Head> heads = new List<Head>();
    public List<Leg> legs = new List<Leg>();
    List<BasePart> nonDetachables = new List<BasePart>();

    int maxHealth;
    int currentHealth;
    float spawnInTime;

    [HideInInspector] public bool canMove = true;

    private void Awake()
    {
        playerPos = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;

        foreach (BasePart part in parts)
        {
            if (part)
            {
                part.CurrentHealth = part.maxHealth;
                part.GenerateCooldown();
            }
            else
            {
                Debug.LogWarning("Part is missing when calculating max health");
            }
        }

        spawnInTime = Time.time;
    }

    void Update()
    {
        foreach (BasePart part in parts)
        {
            //check if part still exists because it may be destroyed while running the loop
            if (part && Time.time < spawnInTime + part.InitialCooldown)
            {
                return;
            }
        }

        // PROCESS:      
        // CALL Action()
        //    ==> IsReady SET TO false
        //        ==> PERFORM ACTION
        //            ==> StartCoroutine(StartCooldown())
        //                ==> IsReady SET TO true
        // REPEAT

        foreach (Body body in bodies)
        {
            if (body && body.IsReady && !body.IsDisabled && PlayerPos)
            {
                body.Action();
            }
        }
        foreach (Hand hand in hands)
        {
            if (hand && hand.IsReady && !hand.IsDisabled && PlayerPos)
            {
                hand.Action();
            }
        }
        foreach (Head head in heads)
        {
            if (head && head.IsReady && !head.IsDisabled && PlayerPos)
            {
                head.Action();
            }
        }
        
        if (canMove && PlayerPos)
        {
            legs[Random.Range(0, legs.Count)].Action();
        }
    }

    public void TakeDamage(int damage)
    {
        //Debug.Log($"{gameObject.name} took {damage} damage");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Robot died");

        //detach all parts
        foreach (BasePart part in nonDetachables)
        {
            part.Detach();
        }

        //spawn new robot
        StartCoroutine(SpawnNewRobot(3f));

        IEnumerator SpawnNewRobot(float delay)
        {
            yield return new WaitForSeconds(delay);

            RobotBuilder.Instance.GenerateRobot();

            DestroyAllParts();
            Destroy(gameObject);
        }

        
    }

    //returns the sum of all max health of parts
    private int GetMaxHealth()
    {
        int maxHp = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            BasePart part;
            transform.GetChild(i).TryGetComponent(out part);
            if (part != null)
            {
                maxHp += part.maxHealth;
            }
        }

        return maxHp;
    }

    public void AddNonDetachablePart(BasePart nonDetachablePart)
    {
        nonDetachables.Add(nonDetachablePart);
    }

    void DestroyAllParts()
    {
        if (parts.Count == 0)
        {
            return;
        }

        foreach (BasePart part in parts.ToList())
        {
            if (part)
            {
                part.Explode();
            }
        }
    }
}
