using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    //stores all parts
    private BaseRobot baseRobot;

    public BaseRobot Base
    {
        get { return baseRobot; }
        set { baseRobot = value; }
    }

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

        maxHealth = Base.GetMaxHealth();
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
}
