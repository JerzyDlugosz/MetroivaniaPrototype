using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode, InitializeOnLoad]
public class SelectedTilemap
{
    static SelectedTilemap()
    {
        Tilemap.tilemapTileChanged += delegate (Tilemap tilemap, Tilemap.SyncTile[] tiles) {
            tilemap.GetComponentInParent<CustomTilemapData>().wasEdited = true;
        };
        Debug.Log("Up and running");
    }
}
