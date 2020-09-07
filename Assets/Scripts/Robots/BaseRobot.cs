using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRobot
{
    public Head head;
    public Body body;
    public Hand[] hands;
    public Leg[] legs;

    public BaseRobot(Head _head, Body _body, Hand[] _hands, Leg[] _legs)
    {
        head = _head;
        body = _body;
        hands = _hands;
        legs = _legs;
    }

    public int GetMaxHealth()
    {
        int maxHp = 0;

        List<BasePart> parts = RobotBuilder.Instance.GetAllParts(this);

        foreach (BasePart part  in parts)
        {
            maxHp += part.health;
        }

        return maxHp;
    }
}
