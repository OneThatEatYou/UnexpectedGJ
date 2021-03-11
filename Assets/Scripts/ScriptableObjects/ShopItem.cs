using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "ScriptableObjects/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public int itemPrice;
    [TextArea]
    public string itemDescription;
    public AnimationClip animationClip;
}
