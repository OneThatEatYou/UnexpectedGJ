using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRobot
{
    [HideInInspector] public Head head;
    [HideInInspector] public Body body;
    [HideInInspector] public Hand[] hands;
    [HideInInspector] public Leg[] legs;

    public BaseRobot(Head _head, Body _body, Hand[] _hands, Leg[] _legs)
    {
        head = _head;
        body = _body;
        hands = _hands;
        legs = _legs;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
