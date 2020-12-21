using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPreview(typeof(SpawnablePart))]
public class SpawnablePartPreview : ObjectPreview
{
    public override bool HasPreviewGUI()
    {
        return true;
    }
}
