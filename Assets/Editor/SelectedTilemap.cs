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
            if(tilemap.transform.parent != null)
            {
                if (tilemap.transform.parent.TryGetComponent(out CustomTilemapData customTilemapData))
                {
                    customTilemapData.wasEdited = true;
                    customTilemapData.isPlayable = true;
                }
            }
        };
        Debug.Log("Up and running");
    }
}
