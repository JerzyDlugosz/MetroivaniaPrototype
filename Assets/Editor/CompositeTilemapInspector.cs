using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CompositeTilemap))]
[CanEditMultipleObjects]
public class CompositeTilemapInspector : Editor
{
    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        DrawDefaultInspector();

        CompositeTilemap compositeTilemapScript = (CompositeTilemap)target;

        if (GUILayout.Button("Set connected tilemaps"))
        {
            compositeTilemapScript.SetConnectedTilemaps();
        }
    }
}
