using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject tilemapTemplatePrefab;

    public List<GameObject> currentTilemaps;

    private CustomTilemapData tilemapData;

    [SerializeField]
    private Vector2Int tilemapSize;

    [SerializeField]
    private List<NoiseData> noiseData;

    [SerializeField]
    private Vector2Int mapSize;

    [SerializeField]
    private List<TileBase> wallTiles;

    [SerializeField]
    private TileBase wallHelperTile;

    private int[,] mapArray;
    private int[,] mapCornerArray;

    [SerializeField]
    Transform tempParent;

    [SerializeField]
    private string folderPath = "Assets/GeneratedMap";

    private string mapFolder = "Maps";
    private string prefabFolder = "Prefabs";

    public void CreateMap()
    {
        string path = $"{folderPath}/{mapFolder}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder($"{folderPath}", $"{mapFolder}");
        }
        MapList mapList = ScriptableObject.CreateInstance<MapList>();

        string path2 = $"{folderPath}/{prefabFolder}";
        if (!AssetDatabase.IsValidFolder(path2))
        {
            AssetDatabase.CreateFolder($"{folderPath}", $"{prefabFolder}");
        }

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                currentTilemaps.Add(Instantiate(tilemapTemplatePrefab, new Vector3(i * tilemapSize.x, j * tilemapSize.y, 1f), Quaternion.identity));
                tilemapData = currentTilemaps[currentTilemaps.Count - 1].GetComponent<CustomTilemapData>();

                Tilemap wallsTilemap = tilemapData.WallsTilemap;
                wallsTilemap.size = new Vector3Int(tilemapSize.x, tilemapSize.y, 1);
                wallsTilemap.origin = new Vector3Int(-(tilemapSize.x/2), -(tilemapSize.y/2), 1);


                Tilemap foregroundTilemap = tilemapData.ForegroundTilemap;
                foregroundTilemap.size = new Vector3Int(tilemapSize.x, tilemapSize.y, 1);
                foregroundTilemap.origin = new Vector3Int(-(tilemapSize.x / 2), -(tilemapSize.y / 2), 1);

                GenerateValues(wallsTilemap, foregroundTilemap);
                FillMap(wallsTilemap, foregroundTilemap);

                string assetPath2 = $"{folderPath}/{prefabFolder}/testPrefab[{i} {j}].prefab";

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(currentTilemaps[currentTilemaps.Count - 1], assetPath2);

                Map map = ScriptableObject.CreateInstance<Map>();
                map.mapPrefab = prefab;

                CheckForDoors(map);

                string assetPath = $"{folderPath}/{mapFolder}/testMap[{i} {j}].asset";
                AssetDatabase.CreateAsset(map, assetPath);

                mapList.maps.Add(map);

                currentTilemaps[currentTilemaps.Count - 1].transform.SetParent(tempParent);
            }
        }
        string maplistAssetPath = $"{folderPath}/MapList.asset";
        AssetDatabase.CreateAsset(mapList, maplistAssetPath);
    }

    void GenerateValues(Tilemap wallsTilemap, Tilemap foregroundTilemap)
    {

        mapArray = new int[wallsTilemap.cellBounds.size.x, wallsTilemap.cellBounds.size.y];
        for (int k = 0; k < mapArray.Length; k++)
        {
            mapArray[k % wallsTilemap.cellBounds.size.x, k / wallsTilemap.cellBounds.size.x] = 1;
        }

        mapCornerArray = new int[foregroundTilemap.cellBounds.size.x, foregroundTilemap.cellBounds.size.y];

        float random;
        int perlinNoise;

        int i = 0;

        foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
        {
            float x = tilemapData.transform.position.x * noiseData[0].noiseScale.x;
            float y = tilemapData.transform.position.y * noiseData[0].noiseScale.y;

            perlinNoise = (int)(Mathf.Clamp01(Mathf.PerlinNoise(pos.x * noiseData[0].noiseScale.x + noiseData[0].noiseOffset.x + x,
                pos.y * noiseData[0].noiseScale.y + noiseData[0].noiseOffset.y + y) * wallTiles.Count));

            mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] = perlinNoise;

            i++;
        }

        if (noiseData.Count > 1)
        {
            for (int j = 1; j < noiseData.Count; j++)
            {
                i = 0;
                foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
                {
                    float x = tilemapData.transform.position.x * noiseData[j].noiseScale.x;
                    float y = tilemapData.transform.position.y * noiseData[j].noiseScale.y;

                    if (mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] > 0)
                    {
                        perlinNoise = (int)(Mathf.Clamp01(Mathf.PerlinNoise(pos.x * noiseData[j].noiseScale.x + noiseData[j].noiseOffset.x + x,
                            pos.y * noiseData[j].noiseScale.y + noiseData[j].noiseOffset.y + y) * wallTiles.Count));

                        mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] = perlinNoise;
                    }
                    i++;
                }
            }
        }

        GenerateValuesForForegroundWall();

    }

    private void GenerateValuesForForegroundWall()
    {
        for (int i = 0; i < tilemapSize.x; i++)
        {
            if (mapArray[i,0] > 0)
            {
                mapCornerArray[i, 0] = 1;
            }
            if (mapArray[i, tilemapSize.y - 1] > 0)
            {
                mapCornerArray[i, tilemapSize.y - 1] = 1;
            }
        }

        for (int i = 0; i < tilemapSize.y; i++)
        {
            if (mapArray[0, i] > 0)
            {
                mapCornerArray[0, i] = 1;
            }
            if (mapArray[tilemapSize.x - 1, i] > 0)
            {
                mapCornerArray[tilemapSize.x - 1, i] = 1;
            }
        }
    }

    private void CheckForDoors(Map map)
    {
        for (int i = 1; i < tilemapSize.x - 1; i++)
        {
            if (mapArray[i, 0] == 0)
            {
                map.mapDoors.Add(Direction.Down);
                break;
            }
        }
        for (int i = 1; i < tilemapSize.x - 1; i++)
        {
            if (mapArray[i, tilemapSize.y - 1] == 0)
            {
                map.mapDoors.Add(Direction.Up);
                break;
            }
        }

        for (int i = 1; i < tilemapSize.y - 1; i++)
        {
            if (mapArray[0, i] == 0)
            {
                map.mapDoors.Add(Direction.Left);
                break;
            }
        }
        for (int i = 1; i < tilemapSize.y - 1; i++)
        {
            if (mapArray[tilemapSize.x - 1, i] == 0)
            {
                map.mapDoors.Add(Direction.Right);
                break;
            }
        }
    }

    private void FillMap(Tilemap wallsTilemap, Tilemap foregroundTilemap)
    {
        int i = 0;
        foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
        {
            wallsTilemap.SetTile(pos, wallTiles[mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x]]);
            i++;
        }

        i = 0;
        foreach (var pos in foregroundTilemap.cellBounds.allPositionsWithin)
        {
            if (mapCornerArray[i % foregroundTilemap.cellBounds.size.x, i / foregroundTilemap.cellBounds.size.x] == 1)
            {
                foregroundTilemap.SetTile(pos, wallHelperTile);
            }

            i++;
        }
    }

    public void EditCurrentTilemap()
    {
        ClearGeneratedMaps();
        CreateMap();
    }

    public void GetMapArray()
    {
        string temp = "";

        foreach (var item in mapArray)
        {
            temp += item.ToString() + " ";
        }

        Debug.Log(temp);
    }

    public void ClearGeneratedMaps()
    {
        foreach (GameObject tilemap in currentTilemaps)
        {
            DestroyImmediate(tilemap);
        }
        currentTilemaps.Clear();

        string path = $"{folderPath}/{mapFolder}";
        AssetDatabase.DeleteAsset(path);

        path = $"{folderPath}/MapList.asset";
        AssetDatabase.DeleteAsset(path);

        path = $"{folderPath}/{prefabFolder}";
        AssetDatabase.DeleteAsset(path);
    }
}

[Serializable]
public class NoiseData
{
    public Vector2 noiseScale;
    public Vector2 noiseOffset;
}

