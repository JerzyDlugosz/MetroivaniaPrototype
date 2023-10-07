using UnityEditor;
using UnityEngine;

public class TilemapSelector : EditorWindow
{
    private int[] selectionIDs;

    [MenuItem("Example/Selection")]
    private static void Init()
    {
        TilemapSelector window = (TilemapSelector)GetWindow(typeof(TilemapSelector));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.TextArea(selectionIDs[0].ToString());
        //if (GUILayout.Button("Save"))
        //    SaveSelection();

        //if (GUILayout.Button("Load"))
        //    LoadLastSavedSelection();
    }

    void OnSelectionChange()
    {
        selectionIDs = Selection.instanceIDs;
        GetSelectedTile();
    }

    void GetSelectedTile()
    {
        //foreach (var item in Selection.gameObjects)
        //{
        //    if(item.TryGetComponent<Tilemap>(out Tilemap tilemap))
        //    {
        //        Debug.Log(tilemap.name);

        //        Tilemap.tilemapTileChanged
        //        var pos = GridSelection.position;
        //        var tile = tilemap.GetTile(pos.position);
        //        Debug.Log(tile.name);
                
        //    }
        //}

        //if (tilemap == null)
        //    return;

        //if (Selection.activeObject != null)
        //{
        //    var gridSel = GridSelection.active;

        //    if (gridSel)
        //    {
        //        pos = GridSelection.position;
        //        tile = tilemap.GetTile(pos.position);
        //    }
        //}
    }

}
