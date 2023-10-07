using System.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CustomTilemap))]
[CanEditMultipleObjects]
public class TilemapInspector : Editor
{
    void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        DrawDefaultInspector();

        CustomTilemap customTilemapScript = (CustomTilemap)target;
        if (GUILayout.Button("SetupTilemapBorders"))
        {
            customTilemapScript.FirstSetupMap();
        }

        serializedObject.ApplyModifiedProperties();
    }

}
