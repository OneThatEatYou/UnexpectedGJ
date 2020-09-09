using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : ScriptableObject
{
    public string partName;
    public int health;

    private PartController partController;
    public PartController PartController 
    {
        get { return partController; }
        set { partController = value; }
    }

    public virtual void Action()
    {
        
    }
}
