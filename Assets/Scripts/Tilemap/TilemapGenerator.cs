using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if (UNITY_EDITOR)

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField]
    private GameManagerScript gameManagerScript;
    [SerializeField]
    private EditorScripts editorScripts;
    [SerializeField]
    private MapList tilemapGenMapList;
    [SerializeField]
    private GameObject tilemapTemplatePrefab;
    [SerializeField]
    private GameObject additionalGameObjectPrefab;

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
    private List<TileBase> backgroundTiles;
    [SerializeField]
    private List<TileBase> groupAnimationTiles;

    [SerializeField]
    private TileBase zone1WallHelperTile;
    [SerializeField]
    private TileBase zone2WallHelperTile;
    [SerializeField]
    private TileBase zone3WallHelperTile;

    private byte[,] mapArray;
    private byte[,] mapCornerArray;

    private List<byte[,]> mapArrayHolder;

    /// <summary>
    /// Parent for all editable Tilemaps
    /// </summary>
    [SerializeField]
    private Transform parentForAllTilemaps;

    /// <summary>
    /// Path to where saved assets will be stored (.prefab, .asset)
    /// </summary>
    [SerializeField]
    private string folderPath = "Assets/GeneratedMapZone2";

    /// <summary>
    /// Additional path to where Map scriptable objects will be stored ("folderPath + "/" + mapFolder")
    /// </summary>
    private string mapFolder = "Maps";

    /// <summary>
    /// Additional path to where prefabs will be stored ("folderPath + "/" + prefabFolder")
    /// </summary>
    private string prefabFolder = "Prefabs";

    /// <summary>
    /// Toggle for hidding deletion buttons
    /// </summary>
    public bool LockDeletion = true;

    /// <summary>
    /// Path for adding new gameobjects 
    /// </summary>
    [SerializeField]
    private string newGameObjectPath;

    /// <summary>
    /// Name of a new added gameobject 
    /// </summary>
    [SerializeField]
    private string newGameObjectName;

    /// <summary>
    /// Name of the gameobject with grouped animation for generating random animated tiles
    /// </summary>
    [SerializeField]
    private string groupAnimationGameObjectName;

    /// <summary>
    /// base rotation of tile to add (facing rotation)
    /// 0 - up
    /// 90 - right
    /// 180 - down
    /// 270 - left
    /// </summary>
    [SerializeField]
    private facingRotation tileBaseRotation;

    public void Start()
    {
        if(gameManagerScript == null)
        {
            gameManagerScript = GameManagerScript.instance;
        }
    }

    public void CreateMap()
    {
        mapArrayHolder = new List<byte[,]>();

        //AstarPath.active.data.gridGraph.center = new Vector3((mapSize.x * tilemapSize.x) / 2 - tilemapSize.x / 2, (mapSize.y * tilemapSize.y) / 2 - tilemapSize.y / 2, 1f);
        //AstarPath.active.data.gridGraph.SetDimensions((mapSize.x * tilemapSize.x) + 1, (mapSize.y * tilemapSize.y) + 1, 1f);

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

                mapArrayHolder.Add(mapArray);

                currentTilemaps[currentTilemaps.Count - 1].transform.SetParent(parentForAllTilemaps);

                tilemapData.wasEdited = false;
            }
        }
        SetupFolders();
    }

    void GenerateValues(Tilemap wallsTilemap, Tilemap foregroundTilemap)
    {

        mapArray = new byte[wallsTilemap.cellBounds.size.x, wallsTilemap.cellBounds.size.y];
        for (int k = 0; k < mapArray.Length; k++)
        {
            mapArray[k % wallsTilemap.cellBounds.size.x, k / wallsTilemap.cellBounds.size.x] = 1;
        }

        mapCornerArray = new byte[foregroundTilemap.cellBounds.size.x, foregroundTilemap.cellBounds.size.y];

        int perlinNoise;

        int i = 0;

        foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
        {
            float x = tilemapData.transform.position.x * noiseData[0].noiseScale.x;
            float y = tilemapData.transform.position.y * noiseData[0].noiseScale.y;

            perlinNoise = (int)(Mathf.Clamp01(Mathf.PerlinNoise(pos.x * noiseData[0].noiseScale.x + noiseData[0].noiseOffset.x + x,
                pos.y * noiseData[0].noiseScale.y + noiseData[0].noiseOffset.y + y) * wallTiles.Count));

            mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] = (byte)perlinNoise;

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

                        mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] = (byte)perlinNoise;
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
            if (map.mapTileArray[i, 0] == 0)
            {
                map.mapDoors.Add(Direction.Down);
                break;
            }
        }
        for (int i = 1; i < tilemapSize.x - 1; i++)
        {
            if (map.mapTileArray[i, tilemapSize.y - 1] == 0)
            {
                map.mapDoors.Add(Direction.Up);
                break;
            }
        }

        for (int i = 1; i < tilemapSize.y - 1; i++)
        {
            if (map.mapTileArray[0, i] == 0)
            {
                map.mapDoors.Add(Direction.Left);
                break;
            }
        }
        for (int i = 1; i < tilemapSize.y - 1; i++)
        {
            if (map.mapTileArray[tilemapSize.x - 1, i] == 0)
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
                foregroundTilemap.SetTile(pos, zone1WallHelperTile);
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

        //Debug.Log(temp);
    }



    #region FolderSaving

    public void ClearGeneratedMaps()
    {
        foreach (GameObject tilemap in currentTilemaps)
        {
            DestroyImmediate(tilemap);
        }
        currentTilemaps.Clear();
        mapArrayHolder.Clear();

        ClearPrefabs();
    }

    public void ClearPrefabs()
    {
        string path = $"{folderPath}/{mapFolder}";
        AssetDatabase.DeleteAsset(path);

        path = $"{folderPath}/MapList.asset";
        AssetDatabase.DeleteAsset(path);

        path = $"{folderPath}/{prefabFolder}";
        AssetDatabase.DeleteAsset(path);
    }

    private GameObject CreatePrefabs(GameObject currentMap, int index)
    {
        string assetPath = $"{folderPath}/{prefabFolder}/[{index / mapSize.x},{index % mapSize.x}].prefab";

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(currentMap, assetPath);

        return prefab;
    }

    private Map CreateMaps(GameObject prefab, int index, int nameIndex)
    {
        Map map = ScriptableObject.CreateInstance<Map>();
        map.mapPrefab = prefab;
        map.mapID = index;
        map.mapTileArray = mapArrayHolder[index];

        CheckForDoors(map);

        string assetPath = $"{folderPath}/{mapFolder}/[{nameIndex / mapSize.x},{nameIndex % mapSize.x}].asset";
        AssetDatabase.CreateAsset(map, assetPath);

        return map;
    }

    public void CreateFolders()
    {
        string path = $"{folderPath}/{mapFolder}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder($"{folderPath}", $"{mapFolder}");
        }

        path = $"{folderPath}/{prefabFolder}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder($"{folderPath}", $"{prefabFolder}");
        }
    }

    private void SetupFolders()
    {
        try
        {
            AssetDatabase.StartAssetEditing();

            CreateFolders();
            int i = 0;
            foreach (var item in currentTilemaps)
            {
                CustomTilemapData tilemapData = item.GetComponent<CustomTilemapData>();
                item.name = $"[{i / mapSize.x},{i % mapSize.x}]";
                if (!tilemapData.isPlayable)
                {
                    i++;
                    continue;
                }

                CreatePrefabs(item, i);
                i++;
            }
        }
        catch (Exception exc)
        {
            Debug.LogError(exc.ToString());
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        List<GameObject> prefabs = new List<GameObject>();

        string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { $"{folderPath}/{prefabFolder}" });

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            prefabs.Add(prefab);
        }

        try
        {
            AssetDatabase.StartAssetEditing();
            MapList mapList = ScriptableObject.CreateInstance<MapList>();
            int i = 0;
            int j = 0;
            foreach (var item in currentTilemaps)
            {
                CustomTilemapData tilemapData = item.GetComponent<CustomTilemapData>();

                //Debug.Log(i);
                if (!tilemapData.isPlayable)
                {
                    mapList.maps.Add(null);
                    j++;
                    continue;
                }

                mapList.maps.Add(CreateMaps(prefabs[i], i, j));
                i++;
                j++;
            }

            string maplistAssetPath = $"{folderPath}/MapList.asset";
            AssetDatabase.CreateAsset(mapList, maplistAssetPath);
        }
        catch (Exception exc)
        {
            Debug.LogError(exc.ToString());
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        guids = AssetDatabase.FindAssets("MapList", new string[] { $"{folderPath}" });

        MapList maplist = ScriptableObject.CreateInstance<MapList>();
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            maplist = AssetDatabase.LoadAssetAtPath<MapList>(path);
        }


        tilemapGenMapList = maplist;
        gameManagerScript.currentMapList = maplist;
    }
    #endregion


    public void RegenerateForegroundMap()
    {
        mapArrayHolder = new List<byte[,]>();
        int numberOfEditedTilemaps = 0;

        foreach (var item in currentTilemaps)
        {
            CustomTilemapData tilemapData = item.GetComponent<CustomTilemapData>();

            if(!tilemapData.isPlayable)
            {
                continue;
            }
            if (tilemapData.wasEdited)
            {
                numberOfEditedTilemaps++;
            }

            Tilemap wallsTilemap = tilemapData.WallsTilemap;
            Tilemap foregroundTilemap = tilemapData.ForegroundTilemap;


            wallsTilemap.RefreshAllTiles();
            foregroundTilemap.RefreshAllTiles();


            mapArray = new byte[wallsTilemap.cellBounds.size.x, wallsTilemap.cellBounds.size.y];
            for (int k = 0; k < mapArray.Length; k++)
            {
                mapArray[k % wallsTilemap.cellBounds.size.x, k / wallsTilemap.cellBounds.size.x] = 1;
            }

            mapCornerArray = new byte[foregroundTilemap.cellBounds.size.x, foregroundTilemap.cellBounds.size.y];

            int i = 0;

            //Debug.Log($"x,y: {wallsTilemap.cellBounds.size.x}, {wallsTilemap.cellBounds.size.y}");
            foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
            {
                //Debug.Log($"{pos.x}, {pos.y}, {pos.z}");
                //Debug.Log($"{i % wallsTilemap.cellBounds.size.x}, {i / wallsTilemap.cellBounds.size.y}");


                int a = Math.Abs(i % wallsTilemap.cellBounds.size.x);
                int b = Math.Abs(i / wallsTilemap.cellBounds.size.y);

                //if (i / wallsTilemap.cellBounds.size.x >= wallsTilemap.cellBounds.size.x)
                //{
                //    Debug.LogWarning($"{pos.x}, {pos.y}, {pos.z}");
                //    Debug.LogWarning($"{mapArray.Length}");
                //    Debug.LogWarning($"x,x: {wallsTilemap.cellBounds.size.x}, {wallsTilemap.cellBounds.size.x}");
                //    Debug.LogWarning($"x,y: {wallsTilemap.cellBounds.size.x}, {wallsTilemap.cellBounds.size.y}");
                //    Debug.LogWarning($"{i % wallsTilemap.cellBounds.size.x}, {i / wallsTilemap.cellBounds.size.y}");

                //    Debug.LogWarning($"a: {a}");
                //    Debug.LogWarning($"b: {b}");
                //}

                if (wallsTilemap.GetTile(pos) != wallTiles[0])
                {
                    mapArray[a, b] = 1;
                }
                else
                {
                    mapArray[a, b] = 0;
                }
                i++;
            }

            GenerateValuesForForegroundWall();

            i = 0;
            foreach (var pos in foregroundTilemap.cellBounds.allPositionsWithin)
            {
                foregroundTilemap.SetTile(pos, null);
                try
                {

                    if (mapCornerArray[i % foregroundTilemap.cellBounds.size.x, i / foregroundTilemap.cellBounds.size.x] == 1)
                    {
                        if(item.GetComponent<CustomTilemap>().zone == Zone.Zone1)
                        {
                            foregroundTilemap.SetTile(pos, zone1WallHelperTile);
                        }

                        if (item.GetComponent<CustomTilemap>().zone == Zone.Zone2)
                        {
                            foregroundTilemap.SetTile(pos, zone2WallHelperTile);
                        }

                        if (item.GetComponent<CustomTilemap>().zone == Zone.Zone3)
                        {
                            foregroundTilemap.SetTile(pos, zone3WallHelperTile);
                        }

                        if (item.GetComponent<CustomTilemap>().zone == Zone.Menu)
                        {
                            foregroundTilemap.SetTile(pos, zone1WallHelperTile);
                        }

                        if (item.GetComponent<CustomTilemap>().zone == Zone.SpecialZone)
                        {
                            foregroundTilemap.SetTile(pos, zone1WallHelperTile);
                        }

                        if (item.GetComponent<CustomTilemap>().zone == Zone.SpecialBoss)
                        {
                            foregroundTilemap.SetTile(pos, zone1WallHelperTile);
                        }

                    }
                }
                catch (Exception exc)
                {

                    Debug.LogError(exc);
                }

                i++;
            }
            mapArrayHolder.Add(mapArray);

            tilemapData.wasEdited = false;
        }

        Debug.Log($"edited {numberOfEditedTilemaps} Tilemaps");

        if(!gameManagerScript.isMainMenu)
        {
            editorScripts.SetStatCollectibleDatanCollectibleList();
        }

        ClearPrefabs();

        SetupFolders();

        SetupCompositeMaps();
    }

    public void ApplyPrefabChanges()
    {
        if (currentTilemaps[0].transform.Find($"{additionalGameObjectPrefab.name}(Clone)"))
        {
            Debug.Log($"{currentTilemaps[0].name} already has this prefab. Skipping adding prefab to every other GameObject");
            return;
        }
        foreach (var item in currentTilemaps)
        {
            Instantiate(additionalGameObjectPrefab, item.transform);
        }
    }

    public void SetupCompositeMaps()
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab", new string[] { $"{folderPath}/{prefabFolder}" });


        EditorUtility.SetDirty(tilemapGenMapList);
        EditorUtility.SetDirty(gameManagerScript.currentMapList);


        foreach (var item in gameManagerScript.currentMapList.maps)
        {
            if (item == null)
            {
                continue;
            }
            if (item.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
            {
                comp.connectedTilemaps.Clear();
            }
        }

        int i = 0;

        foreach (var item in gameManagerScript.currentMapList.maps)
        {
            if (item == null)
            {
                continue;
            }
            if (item.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
            {
                //var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //CompositeTilemap compPrefab = prefab.GetComponent<CompositeTilemap>();

                //comp.connectedTilemaps.Clear();

                for (int j = comp.compMapXPos.x; j < comp.compMapXPos.y + 1; j++)
                {
                    for (int k = comp.compMapYPos.x; k < comp.compMapYPos.y + 1; k++)
                    {
                        //Debug.Log($"{i},{compPrefab}");
                        //Debug.Log((j * mapSize.x) + k);
                        comp.connectedTilemaps.Add(tilemapGenMapList.maps[(j * mapSize.x) + k]);
                        //compPrefab.connectedTilemaps.Add(gameManagerScript.currentMapList.maps[(j * mapSize.x) + k]);
                        comp.compMapXSize = comp.compMapXPos.y - comp.compMapXPos.x + 1;
                        comp.compMapYSize = comp.compMapYPos.y - comp.compMapYPos.x + 1;

                    }
                }
            }
            i++;
        }

        //for (int i = 0; i < currentTilemaps.Count; i++)
        //{
        //    if (currentTilemaps[i].TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
        //    {
        //        var path = AssetDatabase.GUIDToAssetPath(guids[i]);
        //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //        CompositeTilemap compPrefab = prefab.GetComponent<CompositeTilemap>();

        //        comp.connectedTilemaps.Clear();

        //        for (int j = comp.compMapXPos.x; j < comp.compMapXPos.y + 1; j++)
        //        {
        //            for (int k = comp.compMapYPos.x; k < comp.compMapYPos.y + 1; k++)
        //            {
        //                Debug.Log(compPrefab);
        //                Debug.Log((j * mapSize.x) + k);
        //                comp.connectedTilemaps.Add(gameManagerScript.currentMapList.maps[(j * mapSize.x) + k]);
        //                compPrefab.connectedTilemaps.Add(gameManagerScript.currentMapList.maps[(j * mapSize.x) + k]);
        //                compPrefab.compMapXSize = compPrefab.compMapXPos.y - compPrefab.compMapXPos.x + 1;
        //                compPrefab.compMapYSize = compPrefab.compMapYPos.y - compPrefab.compMapYPos.x + 1;

        //            }
        //        }
        //    }
        //}

        RecheckDoorsForCompositeMaps();
    }

    private void RecheckDoorsForCompositeMaps()
    {

        int i = 0;
        foreach (var item in tilemapGenMapList.maps)
        {
            if(item == null)
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

                foreach (var item2 in tilemapGenMapList.maps[i].mapDoors)
                {
                    //Debug.Log($"1. [{i/20}.{i%20}] {item2}");
                }

                if (comp.connectedTilemaps.Contains(tilemapGenMapList.maps[temp[0]]))
                    tilemapGenMapList.maps[i].mapDoors.Remove(Direction.Down);

                if (comp.connectedTilemaps.Contains(tilemapGenMapList.maps[temp[1]]))
                    tilemapGenMapList.maps[i].mapDoors.Remove(Direction.Up);

                if (comp.connectedTilemaps.Contains(tilemapGenMapList.maps[temp[2]]))
                    tilemapGenMapList.maps[i].mapDoors.Remove(Direction.Left);

                if (comp.connectedTilemaps.Contains(tilemapGenMapList.maps[temp[3]]))
                    tilemapGenMapList.maps[i].mapDoors.Remove(Direction.Right);

                foreach (var item2 in tilemapGenMapList.maps[i].mapDoors)
                {
                    //Debug.Log($"2. [{i / 20}.{i % 20}] {item2}");
                }
            }
            i++;
        }

        //for (int i = 0; i < currentTilemaps.Count; i++)
        //{
        //    if (currentTilemaps[i].TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
        //    {
        //        List<Direction> directions = new List<Direction>
        //        {
        //            Direction.Up,
        //            Direction.Down,
        //            Direction.Left,
        //            Direction.Right
        //        };

        //        int[] temp =
        //        {
        //            (i - 1) % (mapSize.x * mapSize.y),
        //            (i + 1) % (mapSize.x * mapSize.y),
        //            (i - mapSize.y) % (mapSize.x * mapSize.y),
        //            (i + mapSize.y) % (mapSize.x * mapSize.y)
        //        };

        //        foreach (var item in gameManagerScript.currentMapList.maps[i].mapDoors)
        //        {
        //            Debug.Log($"1. {item}");
        //        }

        //        if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[0]]))
        //            gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Down);

        //        if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[1]]))
        //            gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Up);

        //        if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[2]]))
        //            gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Left);

        //        if (comp.connectedTilemaps.Contains(gameManagerScript.currentMapList.maps[temp[3]]))
        //            gameManagerScript.currentMapList.maps[i].mapDoors.Remove(Direction.Right);

        //        foreach (var item in gameManagerScript.currentMapList.maps[i].mapDoors)
        //        {
        //            Debug.Log($"2. {item}");
        //        }
        //    }
        //}
    }

    public void RegenerateBackgroundMap()
    {
        //RegenerateMap("Background_Wall", backgroundTiles);

        for (int k = 0; k < currentTilemaps.Count; k++)
        {
            tilemapData = currentTilemaps[k].GetComponent<CustomTilemapData>();

            Tilemap backgroundTilemap = tilemapData.BackgroundTilemap;
            backgroundTilemap.size = new Vector3Int(tilemapSize.x + 2, tilemapSize.y + 2, 1);
            //Changed from: new Vector3Int(tilemapSize.x, tilemapSize.y, 1)

            backgroundTilemap.origin = new Vector3Int(-((tilemapSize.x / 2) + 1), -((tilemapSize.y / 2) + 1), 1);
            //Changed from: new Vector3Int(-(tilemapSize.x / 2), -(tilemapSize.y / 2), 1)

            mapArray = new byte[backgroundTilemap.cellBounds.size.x, backgroundTilemap.cellBounds.size.y];
            for (int l = 0; l < mapArray.Length; l++)
            {
                mapArray[l % backgroundTilemap.cellBounds.size.x, l / backgroundTilemap.cellBounds.size.x] = 0;
            }

            int i;
            int perlinNoise;

            for (int j = 1; j < noiseData.Count; j++)
            {
                i = 0;
                foreach (var pos in backgroundTilemap.cellBounds.allPositionsWithin)
                {
                    float x = tilemapData.transform.position.x * noiseData[j].noiseScale.x;
                    float y = tilemapData.transform.position.y * noiseData[j].noiseScale.y;


                    if (mapArray[i % backgroundTilemap.cellBounds.size.x, i / backgroundTilemap.cellBounds.size.x] > 0)
                    {
                        perlinNoise = (int)(Mathf.Clamp01(Mathf.PerlinNoise(pos.x * noiseData[j].noiseScale.x + noiseData[j].noiseOffset.x + x,
                            pos.y * noiseData[j].noiseScale.y + noiseData[j].noiseOffset.y + y) * backgroundTiles.Count));

                        mapArray[i % backgroundTilemap.cellBounds.size.x, i / backgroundTilemap.cellBounds.size.x] = (byte)perlinNoise;
                    }
                    i++;
                }
            }

            i = 0;
            foreach (var pos in backgroundTilemap.cellBounds.allPositionsWithin)
            {
                backgroundTilemap.SetTile(pos, backgroundTiles[mapArray[i % backgroundTilemap.cellBounds.size.x, i / backgroundTilemap.cellBounds.size.x]]);
                i++;
            }
        }
    }

    public void RegenerateMap(string GameobjectName, List<TileBase> tiles)
    {
        for (int k = 0; k < currentTilemaps.Count; k++)
        {
            Transform foundGameobject = currentTilemaps[k].transform.Find(GameobjectName);

            if(foundGameobject == null) 
            {
                Debug.LogWarning($"Cannot find {GameobjectName} gameobject in currentTilemaps!");
            }

            if(foundGameobject != null)
            {
                Tilemap gameobjectTilemap = foundGameobject.GetComponent<Tilemap>();

                mapArray = new byte[gameobjectTilemap.cellBounds.size.x, gameobjectTilemap.cellBounds.size.y];
                for (int l = 0; l < mapArray.Length; l++)
                {
                    mapArray[l % gameobjectTilemap.cellBounds.size.x, l / gameobjectTilemap.cellBounds.size.x] = 0;
                }

                int i;
                int perlinNoise;

                for (int j = 1; j < noiseData.Count; j++)
                {
                    i = 0;
                    foreach (var pos in gameobjectTilemap.cellBounds.allPositionsWithin)
                    {
                        float x = tilemapData.transform.position.x * noiseData[j].noiseScale.x;
                        float y = tilemapData.transform.position.y * noiseData[j].noiseScale.y;

                        perlinNoise = (int)(Mathf.Clamp01(Mathf.PerlinNoise(pos.x * noiseData[j].noiseScale.x + noiseData[j].noiseOffset.x + x,
                            pos.y * noiseData[j].noiseScale.y + noiseData[j].noiseOffset.y + y) * backgroundTiles.Count));

                        mapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x] = (byte)perlinNoise;

                        i++;
                    }
                }

                i = 0;
                foreach (var pos in gameobjectTilemap.cellBounds.allPositionsWithin)
                {
                    gameobjectTilemap.SetTile(pos, tiles[mapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x]]);
                    i++;
                }
            }
        }
    }

    public void RegenerateGroupAnimationMap()
    {
        //this was changed to work for any map, not the animation map. Change some things for it to work for animation map

        string GameobjectName = groupAnimationGameObjectName;
        List<TileBase> tileBases = groupAnimationTiles;

        //Debug.Log(tileBases.Count);

        for (int k = 0; k < currentTilemaps.Count; k++)
        {
            Transform foundGameobject = currentTilemaps[k].transform.Find(GameobjectName);

            if (foundGameobject == null)
            {
                Debug.LogWarning($"Cannot find {GameobjectName} gameobject in currentTilemaps!");
            }

            if (foundGameobject != null)
            {
                tilemapData = currentTilemaps[k].GetComponent<CustomTilemapData>();

                Tilemap wallsTilemap = tilemapData.WallsTilemap;
                wallsTilemap.size = new Vector3Int(tilemapSize.x, tilemapSize.y, 1);
                wallsTilemap.origin = new Vector3Int(-(tilemapSize.x / 2), -(tilemapSize.y / 2), 1);

                Tilemap gameobjectTilemap = foundGameobject.GetComponent<Tilemap>();
                gameobjectTilemap.size = new Vector3Int(tilemapSize.x, tilemapSize.y, 1);
                gameobjectTilemap.origin = new Vector3Int(-(tilemapSize.x / 2), -(tilemapSize.y / 2), 1);

                byte[,] noiseMapArray = new byte[gameobjectTilemap.cellBounds.size.x, gameobjectTilemap.cellBounds.size.y];
                for (int l = 0; l < noiseMapArray.Length; l++)
                {
                    noiseMapArray[l % gameobjectTilemap.cellBounds.size.x, l / gameobjectTilemap.cellBounds.size.x] = 1;
                }

                mapArray = new byte[gameobjectTilemap.cellBounds.size.x, gameobjectTilemap.cellBounds.size.y];
                for (int l = 0; l < mapArray.Length; l++)
                {
                    mapArray[l % gameobjectTilemap.cellBounds.size.x, l / gameobjectTilemap.cellBounds.size.x] = 0;
                }

                int i = 0;
                int perlinNoise;

                foreach (var pos in wallsTilemap.cellBounds.allPositionsWithin)
                {
                    if (wallsTilemap.GetTile(pos) != wallTiles[0])
                    {
                        mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] = 1;
                    }
                    else
                    {
                        mapArray[i % wallsTilemap.cellBounds.size.x, i / wallsTilemap.cellBounds.size.x] = 0;
                    }
                    i++;
                }

                for (int j = 1; j < noiseData.Count; j++)
                {
                    i = 0;
                    foreach (var pos in gameobjectTilemap.cellBounds.allPositionsWithin)
                    {
                        float x = tilemapData.transform.position.x * noiseData[j].noiseScale.x;
                        float y = tilemapData.transform.position.y * noiseData[j].noiseScale.y;

                        if (noiseMapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x] > 0)
                        {
                            perlinNoise = (int)(Mathf.Clamp01(Mathf.PerlinNoise(pos.x * noiseData[j].noiseScale.x + noiseData[j].noiseOffset.x + x,
                                pos.y * noiseData[j].noiseScale.y + noiseData[j].noiseOffset.y + y) * tileBases.Count));

                            noiseMapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x] = (byte)perlinNoise;
                        }
                        i++;
                    }
                }

                //i = 0;
                //foreach (var pos in gameobjectTilemap.cellBounds.allPositionsWithin)
                //{
                //    gameobjectTilemap.SetTile(pos, null);
                //    //noiseMapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x] = 1;
                //    i++;
                //}


                i = 0;
                foreach (var pos in gameobjectTilemap.cellBounds.allPositionsWithin)
                {
                    if (noiseMapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x] > 0)
                    {
                        if (mapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x] == 0)
                        {
                            try
                            {
                                TileBase tile = tileBases[noiseMapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x]];

                                if (mapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x + 1] > 0)
                                {
                                    SetTile(pos, tile, Color.white, (int)(180f - (int)tileBaseRotation), gameobjectTilemap);
                                }

                                //if (mapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x - 1] > 0)
                                //{
                                //    SetTile(pos, tile, Color.white, (int)(0f - (int)tileBaseRotation), gameobjectTilemap);
                                //}

                                //if (mapArray[i % gameobjectTilemap.cellBounds.size.x + 1, i / gameobjectTilemap.cellBounds.size.x] > 0)
                                //{
                                //    SetTile(pos, tile, Color.white, (int)(270f - (int)tileBaseRotation), gameobjectTilemap);
                                //}

                                //if (mapArray[i % gameobjectTilemap.cellBounds.size.x - 1, i / gameobjectTilemap.cellBounds.size.x] > 0)
                                //{
                                //    SetTile(pos, tile, Color.white, (int)(90f - (int)tileBaseRotation), gameobjectTilemap);
                                //    //gameobjectTilemap.SetTile(pos, tileBases[noiseMapArray[i % gameobjectTilemap.cellBounds.size.x, i / gameobjectTilemap.cellBounds.size.x]]);
                                //}
                            }
                            catch(Exception exc)
                            {
                                Debug.LogWarning(exc);
                                Debug.LogWarning("out of bounds, skipping");
                            }
                        }
                    }
                    i++;
                }
            }
        }
    }
    public void UpdateGameObjectsWithNewGameObject()
    {

        foreach (var item in currentTilemaps)
        {
            Transform transform = item.transform.Find($"{newGameObjectPath}/{newGameObjectName}");
            if (transform)
            {
                Debug.Log($"{item.name} already has this gameobject. Skipping adding gameobject");
                continue;
            }
            transform = item.transform.Find(newGameObjectPath);
            GameObject obj = new GameObject(newGameObjectName);
            obj.transform.SetParent(transform);
        }
    }

    private void SetTile(Vector3Int pos, TileBase tile, Color color, Matrix4x4 transform, Tilemap currentTilemap)
    {
        var tileChangeData = new TileChangeData
        {
            position = pos,
            tile = tile,
            color = color,
            transform = transform
        };
        currentTilemap.SetTile(tileChangeData, false);
    }
    private void SetTile(Vector3Int pos, TileBase tile, Color color, int rotation, Tilemap currentTilemap)
    {
        var tileTransform = Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, rotation));
        var tileChangeData = new TileChangeData
        {
            position = pos,
            tile = tile,
            color = color,
            transform = tileTransform
        };
        currentTilemap.SetTile(tileChangeData, false);
    }

    public void SetupAllOutsideColliders()
    {
        foreach (Transform item in parentForAllTilemaps.transform)
        {
            item.GetComponent<CustomTilemap>().FirstSetupMap();
        }
    }

    public void UpdateXandYPosValues()
    {
        foreach (Transform item in parentForAllTilemaps.transform)
        {
            item.GetComponent<CustomTilemap>().UpdatePosValues();
        }
    }

    public void ResetTilemapData()
    {
        foreach (var item in currentTilemaps)
        {
            CustomTilemapData tilemapData = item.GetComponent<CustomTilemapData>();

            Tilemap wallsTilemap = tilemapData.WallsTilemap;
            Tilemap foregroundTilemap = tilemapData.ForegroundTilemap;

            wallsTilemap.size = new Vector3Int(tilemapSize.x, tilemapSize.y, 1);
            wallsTilemap.origin = new Vector3Int(-(tilemapSize.x / 2), -(tilemapSize.y / 2), 1);

            foregroundTilemap.size = new Vector3Int(tilemapSize.x, tilemapSize.y, 1);
            foregroundTilemap.origin = new Vector3Int(-(tilemapSize.x / 2), -(tilemapSize.y / 2), 1);
        }
    }
}

#endif

[Serializable]
public class NoiseData
{
    public Vector2 noiseScale;
    public Vector2 noiseOffset;
}

public enum facingRotation
{
    up = 0,
    right = 90,
    down = 180,
    left = 270,
}
