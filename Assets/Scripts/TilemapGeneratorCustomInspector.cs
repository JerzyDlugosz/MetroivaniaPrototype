using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TilemapGenerator))]
public class TilemapGeneratorCustomInspector : Editor
{
    private bool ClickedGM = false;
    private bool ClickedCM = true;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

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
        if (GUILayout.Button("Regenerate Map"))
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
}
