using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Helmet : ItemController
{
    public int extraHealth = 1;

    private void OnEnable()
    {
        BattleManager.Instance.onRegisterHealthPanel += AddMaxHealth;
    }

    private void OnDisable()
    {
        BattleManager.Instance.onRegisterHealthPanel -= AddMaxHealth;
    }

    void AddMaxHealth()
    {
        BattleManager.Instance.maxHealth += 1;
    }
}
