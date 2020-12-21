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
    public Transform PlayerPos
    { 
        get 
        {
            if (playerPos == null)
            {
                var player = FindObjectOfType<PlayerController>();

                if (player)
                {
                    playerPos = player.transform;
                }
                else
                {
                    return null;
                }
            }
            return playerPos; 
        } 
    }

    public List<BasePart> parts = new List<BasePart>();
    [HideInInspector] public Body body;
    [HideInInspector] public List<Hand> hands = new List<Hand>();
    [HideInInspector] public List<Head> heads = new List<Head>();
    [HideInInspector] public List<Leg> legs = new List<Leg>();
    List<BasePart> nonDetachables = new List<BasePart>();

    [ReadOnly][SerializeField] int maxHealth;
    [ReadOnly][SerializeField] int currentHealth;
    float spawnInTime;

    [HideInInspector] public bool canMove = true;

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

        //actions will still be called when player is dead
        if (body && body.IsReady && !body.IsDisabled)
        {
            body.Action();
        }

        if (hands.Count > 0)
        {
            foreach (Hand hand in hands)
            {
                if (hand && hand.IsReady && !hand.IsDisabled)
                {
                    hand.Action();
                }
            }
        }
        if (heads.Count > 0)
        {
            foreach (Head head in heads)
            {
                if (head && head.IsReady && !head.IsDisabled)
                {
                    head.Action();
                }
            }
        }
        if (legs.Count > 0)
        {
            if (canMove)
            {
                legs[Random.Range(0, legs.Count)].Action();
            }
        }
    }

    public void Initialize()
    {
        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;
        //Debug.Log(currentHealth);

        //set health for each parts
        foreach (BasePart part in parts.ToList())
        {
            if (part)
            {
                part.CurrentHealth = part.maxHealth;
                part.GenerateCooldown();
            }
            else
            {
                Debug.LogWarning("Part is missing when calculating max health");
                parts.Remove(part);
            }
        }

        spawnInTime = Time.time;
    }

    public void AssignPartToPartList(BasePart part)
    {
        parts.Add(part);

        if (part is Body)
        {
            if (body == null)
            {
                body = (Body)part;
            }
            else
            {
                Debug.LogWarning("Multiple bodies found. Only one body will be used.");
            }
        }
        else if (part is Hand)
        {
            hands.Add(part as Hand);
        }
        else if (part is Head)
        {
            heads.Add(part as Head);
        }
        else if (part is Leg)
        {
            legs.Add(part as Leg);
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

        foreach (BasePart part in parts)
        {
            maxHp += part.maxHealth;
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

    public Vector2 GenerateRandomPosition()
    {
        return new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
    }
}
