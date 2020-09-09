using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : ScriptableObject
{
    public string partName;
    public Sprite sprite;
    public int health;

    public virtual void Action()
    {
        
    }
}
