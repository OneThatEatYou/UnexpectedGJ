using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    Transform playerPos;
    public Transform PlayerPos { get { return playerPos; } }

    public List<BasePart> parts = new List<BasePart>();

    int maxHealth;
    int currentHealth;

    private void Awake()
    {
        playerPos = FindObjectOfType<PlayerController>().transform;
    }

    void Start()
    {
        //var parts = RobotBuilder.Instance.GetAllParts(Base);
        //foreach (var item in parts)
        //{
        //    Debug.Log(item.name);
        //}

        //Debug.Log(parts);

        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Robot died");
        //explosion

        //spawn new robot
    }

    public bool CanDetach(BasePart partToDetach)
    {
        BasePart thisPart = null;

        foreach (BasePart part in parts)
        {
            if (partToDetach == part)
            {
                thisPart = part;
                break;
            }
        }

        if (thisPart == null)
        {
            Debug.LogError("Part to detach not found!");
            return false;
        }

        if (thisPart is Body)
        {
            foreach (BasePart part in parts)
            {
                if (!(part is Leg))
                {
                    Debug.Log("Cannot detach body before other parts");
                    return false;
                }
            }

            return true;
        }
        else
        {
            return true;
        }
    }

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
}
