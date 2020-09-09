using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    Transform player;

    int maxHealth;
    int currentHealth;

    void Start()
    {
        //var parts = RobotBuilder.Instance.GetAllParts(Base);
        //foreach (var item in parts)
        //{
        //    Debug.Log(item.name);
        //}

        maxHealth = GetMaxHealth();
        currentHealth = maxHealth;

        player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
