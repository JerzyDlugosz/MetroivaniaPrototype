using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorScripts))]
[CanEditMultipleObjects]
public class EditorScriptsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorScripts EditorScriptsScript = (EditorScripts)target;


        if (GUILayout.Button("Get collectibles and tilemap count"))
        {
            EditorScriptsScript.GetAllCollectiblesCount();
            EditorScriptsScript.GetAllPlayableTilemaps();
        }

        if (GUILayout.Button("Set composite tilemaps"))
        {
            EditorScriptsScript.SetCompositeTilemaps();
        }
    }
}
