using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greenbody : Body
{
    public float cooldownMultiplier;

    private void OnEnable()
    {
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

    private void OnDisable()
    {
        foreach (var part in Controller.parts)
        {
            if (part)
            {
                part.onGenerateCooldown -= ReduceCooldown;
            }
        }
    }
}
