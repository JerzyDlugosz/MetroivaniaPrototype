using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if (UNITY_EDITOR)
public class EditorScripts : MonoBehaviour
{
    [SerializeField]
    private MapList maplist;

    private string folderPath = "Assets/GeneratedMapZone2";
    private string mapFolder = "Maps";
    private string prefabFolder = "Prefabs";

    public void GetAllCollectiblesCount()
    {
        Debug.Log(FindObjectsOfType<CollectibleGameObject>().Length);
    }

    public void GetAllPlayableTilemaps()
    {
        int i = 0;
        foreach (var item in maplist.maps)
        {
            if(item != null)
            {
                i++;
            }
        }
        Debug.Log(i);
    }

    public void SetCompositeTilemaps()
    {
        foreach (Map map in maplist.maps)
        {
            if (map != null)
            {
                if (map.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
                {
                    comp.maplist = maplist;
                    comp.SetConnectedTilemaps();
                }
            }
        }
    }

    public void SetDoors()
    {
        List<Map> maplistTemp = new List<Map>();

        string[] guids = AssetDatabase.FindAssets("t:Map", new string[] { $"{folderPath}/{mapFolder}" });

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            maplistTemp.Add(AssetDatabase.LoadAssetAtPath<Map>(path));
        }

        int guidIndex = 0;

        for (int j = 0; j < 20; j++)
        {
            for (int k = 0; k < 20; k++)
            {

            }
        }




        int i = 0;
        foreach (var item in maplist.maps)
        {
            if (item == null)
            {
                i++;
                continue;
            }
            if (item.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
            {
                List<Direction> directions = new List<Direction>
                {
                    Direction.Up,
                    Direction.Down,
                    Direction.Left,
                    Direction.Right
                };

                int[] temp =
                {
                    (i - 1) % 400,
                    (i + 1) % 400,
                    (i - 20) % 400,
                    (i + 20) % 400
                };

                foreach (var item2 in maplist.maps[i].mapDoors)
                {
                    Debug.Log($"1. [{i / 20}.{i % 20}] {item2}");
                }

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[0]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Down);

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[1]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Up);

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[2]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Left);

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[3]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Right);

                foreach (var item2 in maplist.maps[i].mapDoors)
                {
                    Debug.Log($"2. [{i / 20}.{i % 20}] {item2}");
                }
            }
            i++;
        } 
    }
}
#endif