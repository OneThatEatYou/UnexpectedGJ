using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    public int nuts = 0;
    public HashSet<ShopItem> itemHashset = new HashSet<ShopItem>();
    public GameObject equippedItemObj;
    public ShopItem equippedItem;

    public void OnInit()
    {
        if (Debug.isDebugBuild)
        {
            nuts += 1000;
        }
    }
}
