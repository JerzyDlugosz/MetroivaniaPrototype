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

        //if (GUILayout.Button("Set all collectible data in game"))
        //{
        //    EditorScriptsScript.SetStatCollectibleDataInGame();
        //}

        if (GUILayout.Button("Disable unplayable maps"))
        {
            EditorScriptsScript.SetUnplayableMapVisibility(false);
        }

        if (GUILayout.Button("Enable unplayable maps"))
        {
            EditorScriptsScript.SetUnplayableMapVisibility(true);
        }

        if (GUILayout.Button("Get collectibles positions"))
        {
            EditorScriptsScript.GetAllCollectiblesPositions();
        }

        if (GUILayout.Button("Set stat collectibles data to CollectibleList"))
        {
            EditorScriptsScript.SetStatCollectibleDatanCollectibleList();
        }
    }
}
