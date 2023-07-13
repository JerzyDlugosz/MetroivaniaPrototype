using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class GameManagerScript : MonoBehaviour
{
    public MapList mapList;
    public List<GameObject> previousMaps;
    public List<GameObject> currentMaps;
    public Map[,] mapArray;
    public int[,] directionArray = { {0,1},{1,0},{0,-1 },{-1,0 } };
    private int xSizeMul;
    private int ySizeMul;
    private MinimapController minimap;

    public int centerMapOffset = 100;

    public Player player;
    public static GameManagerScript instance;

    private Vector2 currentPos;

    private Vector2[] forceMoveArray;
    public float forceMoveDistance;

    private int maxTilesInMap;

    private SavingAndLoading savingAndLoading;
    [SerializeField]
    private TilemapGenerator tilemapGenerator;

    [SerializeField]
    private int spawnMapNumber;

    public PlayableMapData playableMapData;

    [SerializeField]
    private int mapSize;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        GlobalData.mapSize = mapSize;
    }
    private void Start()
    {
        savingAndLoading = GameStateManager.instance.GetComponent<SavingAndLoading>();
        maxTilesInMap = GlobalData.maxTilesInMap;
        centerMapOffset = maxTilesInMap/2;

        forceMoveArray = new Vector2[]
            { new Vector2(0, forceMoveDistance), new Vector2(forceMoveDistance, 0), new Vector2(0, -forceMoveDistance), new Vector2(-forceMoveDistance, 0) };
        
        minimap = GetComponent<MinimapController>();
        minimap.SetupMinimap(maxTilesInMap, maxTilesInMap);

        LoadMapsToArray();
        CreateMap(LoadGame(), true);
    }

    private void Update()
    {
        float x = player.transform.position.x + centerMapOffset * GlobalData.mapSize;
        float y = player.transform.position.y + centerMapOffset * GlobalData.mapSize;

        int mapXPos = (int)((x + GlobalData.mapSize / 2) / GlobalData.mapSize);
        int mapYPos = (int)((y + GlobalData.mapSize / 2) / GlobalData.mapSize);

        if (currentPos.x != mapXPos || currentPos.y != mapYPos)
        {
            //int a = (int)Mathf.Abs(currentPos.x) - Mathf.Abs(mapXPos);
            //int b = (int)Mathf.Abs(currentPos.y) - Mathf.Abs(mapYPos);

            if (currentMaps.Count > 1)
            {
                if (currentMaps[0].TryGetComponent(out CompositeTilemap component))
                {
                    minimap.UpdateUnlockedMinimap(mapXPos, mapYPos, mapArray[mapXPos, mapYPos], currentMaps);
                }
                else
                {
                    Debug.LogError("Broked");
                }
            }
            else
            {
                minimap.UpdateUnlockedMinimap(mapXPos, mapYPos, mapArray[mapXPos, mapYPos]);
            }
            currentPos.x = mapXPos;
            currentPos.y = mapYPos;
        }

        //Debug.Log($"x: {mapXPos}/ y:{mapYPos}");
    }


    private void LoadMapsToArray()
    {
        mapArray = new Map[maxTilesInMap, maxTilesInMap];
        int i = 0;
        foreach (Map map in mapList.maps)
        {
            map.mapXOffset = (int)(map.mapPrefab.transform.position.x / GlobalData.mapSize);
            map.mapYOffset = (int)(map.mapPrefab.transform.position.y / GlobalData.mapSize);
            map.mapID = i;
            map.xSize = (int)GlobalData.mapSize;
            map.ySize = (int)GlobalData.mapSize;

            //Debug.Log($"{map.name}: x:{map.mapXOffset} y:{map.mapYOffset}");

            mapArray[centerMapOffset + map.mapXOffset, centerMapOffset + map.mapYOffset] = map;

            i++;
        }

        foreach (Map map in mapList.maps)
        {
            //Debug.Log($"MapArray : [{centerMapOffset + map.mapXOffset} , {centerMapOffset + map.mapYOffset}]");
        }
    }

    public void LoadGameButton()
    {
        LoadGame();
    }

    public void SaveGameButton()
    {
        SaveGame();
    }

    private Map LoadGame()
    {
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);


        player.customCollider.enabled = false;

        if (!savingAndLoading.CheckIfSaveFileExists(save))
        {
            if(spawnMapNumber - 1 > -1 && spawnMapNumber - 1 < mapList.maps.Count)
            {
                Debug.Log($"Spawn Pos:{mapList.maps[spawnMapNumber - 1].mapXOffset * GlobalData.mapSize} ,{mapList.maps[spawnMapNumber - 1].mapYOffset * GlobalData.mapSize}");
                player.characterController.ForceTransportPlayer(new Vector2(mapList.maps[spawnMapNumber - 1].mapXOffset * GlobalData.mapSize, mapList.maps[spawnMapNumber - 1].mapYOffset * GlobalData.mapSize));

                player.customCollider.enabled = true;
                return mapArray[centerMapOffset + mapList.maps[spawnMapNumber - 1].mapXOffset, centerMapOffset + mapList.maps[spawnMapNumber - 1].mapYOffset];
            }

            Debug.LogWarning($"Index out of bounds! spawnMapNumber is set to {spawnMapNumber - 1}. Setting spawn to {centerMapOffset}, {centerMapOffset}");

            player.customCollider.enabled = true;
            return mapArray[centerMapOffset, centerMapOffset];        
        }

        savingAndLoading.LoadGameFile(save);

        minimap.LoadMinimap(save);
        minimap.UpdateMinimap(save.mapXOffset + centerMapOffset, save.mapYOffset + centerMapOffset);

        player.characterController.ForceTransportPlayer(new Vector2(save.mapXOffset * GlobalData.mapSize, save.mapYOffset * GlobalData.mapSize));

        player.customCollider.enabled = true;

        return mapArray[save.mapXOffset + centerMapOffset, save.mapYOffset + centerMapOffset]; //force player to save at [1,1] tile maps

    }


    private void SaveGame()
    {
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);

        if (savingAndLoading.CheckIfSaveFileExists(save))
        {
            
        }

        minimap.SaveMinimap(save);
        save.mapXOffset = mapArray[(int)currentPos.x, (int)currentPos.y].mapXOffset;
        save.mapYOffset = mapArray[(int)currentPos.x, (int)currentPos.y].mapYOffset;
        save.mapID = mapArray[(int)currentPos.x, (int)currentPos.y].mapID;

        savingAndLoading.SaveGameFile(save);
    }

    private void CreateMap(Map currentMap, bool enableTriggers)
    {
        currentMaps.Add(Instantiate(currentMap.mapPrefab));



        currentMaps[0].GetComponent<CustomTilemap>().map = currentMap;
        currentMaps[0].GetComponent<CustomTilemap>().SetupMap();
        if (enableTriggers) currentMaps[0].GetComponent<CustomTilemap>().EnableAllTriggers();

        xSizeMul = 1;
        ySizeMul = 1;

        if (currentMaps[0].TryGetComponent(out CompositeTilemap component))
        {
            int i = 1;
            foreach (Map map in component.connectedTilemaps)
            {
                if (currentMap != map)
                {
                    currentMaps.Add(Instantiate(map.mapPrefab));
                    currentMaps[i].GetComponent<CustomTilemap>().map = map;
                    currentMaps[i].GetComponent<CustomTilemap>().SetupMap();
                    currentMaps[i].GetComponent<CustomTilemap>().EnableAllTriggers();

                    xSizeMul = component.compMapXSize;
                    ySizeMul = component.compMapYSize;

                    i++;
                }
                else
                {
                    Debug.Log("Same map. Skipping");
                }
            }
        }

        SetCameraBoundaries();

        previousMaps = currentMaps;


        foreach (GameObject mapOnScene in currentMaps)
        {
            CustomTilemap tilemap = mapOnScene.GetComponent<CustomTilemap>();

            foreach (Transform childCollider in mapOnScene.GetComponent<CustomTilemap>().outsideColliders.transform)
            {
                try
                {
                    MapBorderCollision mapBorderCollision = childCollider.GetComponent<MapBorderCollision>();

                    foreach (var item in tilemap.map.mapDoors)
                    {
                        if (mapBorderCollision.direction == item)
                        {
                            mapBorderCollision.isADoor = true;
                            break;
                        }
                        else
                        {
                            mapBorderCollision.isADoor = false;
                        }
                    }

                    if(mapBorderCollision.isADoor)
                    {
                        int connectedMapXOffset = centerMapOffset + tilemap.map.mapXOffset + directionArray[(int)mapBorderCollision.direction, 0];
                        int connectedMapYOffset = centerMapOffset + tilemap.map.mapYOffset + directionArray[(int)mapBorderCollision.direction, 1];
                        //Debug.Log($"ChangeMapArray ({mapBorderCollision.direction}): [{connectedMapXOffset}, {connectedMapYOffset}]");

                        mapBorderCollision.onTriggerEnterEvent.AddListener(delegate
                        {
                            ChangeCurrentMap(mapArray[connectedMapXOffset, connectedMapYOffset]);
                            player.characterController.ForceMovePlayer(forceMoveArray[(int)mapBorderCollision.direction]);
                            Camera.main.GetComponent<CameraMovement>().MoveCameraBetweenZones(forceMoveArray[(int)mapBorderCollision.direction]);
                            GameStateManager.instance.audioManager.OnMapChange(mapArray[connectedMapXOffset, connectedMapYOffset]);
                        });


                    }
                    else
                    {
                        mapBorderCollision.onTriggerEnterEvent.AddListener(delegate
                        {
                            Debug.Log("Not a door");
                        });
                    }

                }
                catch (IndexOutOfRangeException exception)
                {
                    Debug.LogError(exception.Message);
                    //MakeAnErrorMap
                }
            }
        }
        
    }

    private void SetCameraBoundaries()
    {
        float xCoord = 0;
        float yCoord = 0;

        foreach (GameObject mapOnScene in currentMaps)
        {
            xCoord = xCoord + (mapOnScene.GetComponent<CustomTilemap>().map.mapXOffset * GlobalData.mapSize);
            yCoord = yCoord + (mapOnScene.GetComponent<CustomTilemap>().map.mapYOffset * GlobalData.mapSize);
        }

        xCoord = NumericExtensions.SafeDivision(xCoord, xSizeMul * ySizeMul);
        yCoord = NumericExtensions.SafeDivision(yCoord, xSizeMul * ySizeMul);

        Camera.main.GetComponent<CameraMovement>().SetCameraCenter(new Vector2(xCoord, yCoord));
        Camera.main.GetComponent<CameraMovement>().MoveCamera(new Vector2(xCoord, yCoord));
        Camera.main.GetComponent<CameraMovement>().SetCameraBoundary(xSizeMul, ySizeMul);
    }

    public void ChangeCurrentMap(Map nextMap)
    {
        foreach (GameObject map in previousMaps)
        {
            Destroy(map);
        }
        previousMaps.Clear();
        currentMaps.Clear();
        CreateMap(nextMap, false);
    }
}


static public class NumericExtensions
{
    static public float SafeDivision(float Numerator, float Denominator)
    {
        return (Denominator == 0) ? 0 : Numerator / Denominator;
    }
}


