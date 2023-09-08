using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TilemapGenerator))]
[CanEditMultipleObjects]
public class TilemapGeneratorCustomInspector : Editor
{
    bool defaultInspectorToggle = true;

    SerializedProperty newGameObjectPath;
    SerializedProperty newGameObjectName;
    SerializedProperty backgroundTiles;
    SerializedProperty additionalGameObjectPrefab;

    void OnEnable()
    {
        newGameObjectPath = serializedObject.FindProperty("newGameObjectPath");
        newGameObjectName = serializedObject.FindProperty("newGameObjectName");
        backgroundTiles = serializedObject.FindProperty("backgroundTiles");
        additionalGameObjectPrefab = serializedObject.FindProperty("additionalGameObjectPrefab");
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        if(defaultInspectorToggle = GUILayout.Toggle(defaultInspectorToggle, "Show default inspector"))
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("For 'Update GameObject (Prefab Changes)' button");
            EditorGUILayout.PropertyField(additionalGameObjectPrefab);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("For 'Update Background' button");
            EditorGUILayout.PropertyField(backgroundTiles);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("For 'Add new gameobject to chosen gameobjects' button");
            EditorGUILayout.PropertyField(newGameObjectPath);
            EditorGUILayout.PropertyField(newGameObjectName);
        }

        TilemapGenerator tilemapGeneratorScript = (TilemapGenerator)target;
        if (GUILayout.Button("Generate Map"))
        {
            if (tilemapGeneratorScript.currentTilemaps.Count == 0)
            {
                tilemapGeneratorScript.CreateMap();
            }
            else
            {
                Debug.Log("Map already Generated");
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Update Map (Tile Changes)"))
        {
            tilemapGeneratorScript.RegenerateForegroundMap();
        }

        if (GUILayout.Button("Update GameObject (Prefab Changes)"))
        {
            tilemapGeneratorScript.ApplyPrefabChanges();
        }

        if (GUILayout.Button("Update CompositeMaps"))
        {
            tilemapGeneratorScript.SetupCompositeMaps();
        }

        //if (GUILayout.Button("Update Background"))
        //{
        //    tilemapGeneratorScript.RegenerateBackgroundMap();
        //}

        EditorGUILayout.LabelField("Dont use is right now, go and see the code");
        if (GUILayout.Button("Update Group Animation Map"))
        {
            tilemapGeneratorScript.RegenerateGroupAnimationMap();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add new gameobject to chosen gameobjects"))
        {
            tilemapGeneratorScript.UpdateGameObjectsWithNewGameObject();
        }

        EditorGUILayout.Space();

        if (!tilemapGeneratorScript.LockDeletion)
        {
            if (GUILayout.Button("Regenerate Noise"))
            {
                tilemapGeneratorScript.EditCurrentTilemap();
                //tilemapGeneratorScript.GetMapArray();
            }

            if (GUILayout.Button("Clear Maps"))
            {
                if (tilemapGeneratorScript.currentTilemaps.Count > 0)
                {
                    tilemapGeneratorScript.ClearGeneratedMaps();
                }
                else
                {
                    Debug.Log("No Map");
                }
            }
        }



        serializedObject.ApplyModifiedProperties();
    }
}
