using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenbody : Body
{
    public float cooldownMultiplier;

    public override void OnEnable()
    {
        base.OnEnable();

        foreach (var part in Controller.parts)
        {
            if (part)
            {
                part.onGenerateCooldown += ReduceCooldown;
            }
        }
    }

    float ReduceCooldown(float cooldown)
    {
        return cooldown * cooldownMultiplier;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        foreach (var part in Controller.parts)
        {
            if (part)
            {
                part.onGenerateCooldown -= ReduceCooldown;
            }
        }
    }
}
