using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : BasePart
{
    public override void Start()
    {
        base.Start();

        Controller.detachables.Add(this);
    }
}
