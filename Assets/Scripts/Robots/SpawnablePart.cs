using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPart", menuName = "SpawnablePart")]
public class SpawnablePart : ScriptableObject
{
    public GameObject partPrefab;
    [Space]
    [ContextMenuItem("Auto Set", "AutoSet")]
    public Sprite sprite;
    public Vector2 LocalPivot { get { return sprite.pivot / sprite.pixelsPerUnit; } }
    public Vector2 ILocalPivot { get { return Size - LocalPivot; } }
    public Vector2 Extents { get { return sprite.bounds.extents; } }
    public Vector2 Size { get { return sprite.bounds.size; } }
    public PartType partType;
}

public enum PartType
{
    Head,
    Body,
    LHand,
    RHand,
    Leg
}
