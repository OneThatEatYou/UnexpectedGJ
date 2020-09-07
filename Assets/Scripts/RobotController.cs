using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    private BaseRobot baseRobot;

    public BaseRobot Base
    {
        get { return baseRobot; }
        set { baseRobot = value; }
    }

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
