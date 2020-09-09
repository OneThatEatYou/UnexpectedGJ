using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHand", menuName = "Parts/Hand/BaseHand", order = 3)]
public class Hand : BasePart
{
    public Sprite leftHandSprite;
    public Sprite rightHandSprite;

    public enum HandPosition
    {
        Left,
        Right
    }
}
