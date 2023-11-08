using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CompositeMapController : MonoBehaviour
{
    [SerializeField]
    private GameManagerScript gameManagerScript;
    [SerializeField]
    private Vector2Int mapSize;

    private string folderPath = "Assets/GeneratedMapZone2";

    private string prefabFolder = "Prefabs";

    public void Start()
    {
        if (gameManagerScript == null)
        {
            gameManagerScript = GameObject.FindObjectOfType<GameManagerScript>();
        }
    }


    public void SetupCompositeMaps()
    {
        //string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { $"{folderPath}/{prefabFolder}" });

        //foreach (var guid in guids)
        //{
        //    var path = AssetDatabase.GUIDToAssetPath(guid);
        //    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        //    if (prefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
        //    {
        //        comp.connectedTilemaps.Clear();
        //    }
        //}
        int i = 0;

        foreach (var item in gameManagerScript.currentMapList.maps)
        {
            if (item == null)
            {
                continue;
            }
            if (item.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
            {
                comp.connectedTilemaps.Clear();


                for (int j = comp.compMapXPos.x; j < comp.compMapXPos.y + 1; j++)
                {
                    for (int k = comp.compMapYPos.x; k < comp.compMapYPos.y + 1; k++)
                    {
                        if (comp == null)
                        {
                            //Debug.LogError($"i, compositeTilemap: {i},{compPrefab}");
                            //Debug.LogError($"mapIndex: {(j * mapSize.x) + k}");
                        }

                        //Debug.Log($"i, compositeTilemap: {i},{compPrefab}");
                        //Debug.Log($"mapIndex: {(j * mapSize.x) + k}");
                        comp.connectedTilemaps.Add(gameManagerScript.currentMapList.maps[(j * mapSize.x) + k]);
                        //compPrefab.connectedTilemaps.Add(gameManagerScript.currentMapList.maps[(j * mapSize.x) + k]);
                        comp.compMapXSize = comp.compMapXPos.y - comp.compMapXPos.x + 1;
                        comp.compMapYSize = comp.compMapYPos.y - comp.compMapYPos.x + 1;

                    }
                }
            }
            i++;
        }

        RecheckDoorsForCompositeMaps();

    }

    private void RecheckDoorsForCompositeMaps()
    {
        int i = 0;
        foreach (var item in gameManagerScript.currentMapList.maps)
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
                    (i - 1) % (mapSize.x * mapSize.y),
                    (i + 1) % (mapSize.x * mapSize.y),
                    (i - mapSize.y) % (mapSize.x * mapSize.y),
                    (i + mapSize.y) % (mapSize.x * mapSize.y)
                };

                foreach (var item2 in gameManagerScript.currentMapList.maps[i].mapDoors)
                {
                    //Debug.Log($"1. {item2}");
                }

                if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[0]]))
                    gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Down);

                if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[1]]))
                    gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Up);

                if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[2]]))
                    gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Left);

                if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[3]]))
                    gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Right);

                foreach (var item2 in gameManagerScript.currentMapList.maps[i].mapDoors)
                {
                    //Debug.Log($"2. {item2}");
                }
            }
            i++;
        }
    }
}