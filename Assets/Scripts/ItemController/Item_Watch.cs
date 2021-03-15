using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Watch : ItemController
{
    public float slowIn;
    public float slowStay;
    public float slowOut;
    public float slowTimeScale;

    public override void UseItem()
    {
        base.UseItem();

        BattleManager.Instance.PlaySlowMo(slowIn, slowStay, slowOut, slowTimeScale);
    }
}
