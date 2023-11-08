using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public List<GameObject> generatedMapsFoldersToHide;
    public MapList currentMapList;
    public List<GameObject> previousMaps;
    public List<GameObject> currentMaps;
    public Map[,] mapArray;
    public int[,] directionArray = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };
    private int xSizeMul;
    private int ySizeMul;
    public MinimapController minimap;

    public int centerMapOffset = 100;

    public Player player;
    public static GameManagerScript instance;

    private Vector2 currentPos;

    private Vector2[] forceMoveArray;
    public float forceMoveDistance;

    private int maxTilesInMap;

    private SavingAndLoading savingAndLoading;

    private int spawnMapNumber;
    [SerializeField]
    private Vector2Int spawnMapPos;

    public PlayableMapData playableMapData;

    [SerializeField]
    private int mapSize;

    [SerializeField]
    private Transform preloadedMapsParent;
    //[SerializeField]
    //private bool preloadMaps;
    [SerializeField]
    private bool areMapsPreloaded;
    [SerializeField]
    private bool isMinimapAvailable = true;
    [SerializeField]
    private bool willLoadSaveFile = true;
    public CameraMovement cameraMovement;

    private GameObject[,] preloadedMaps;

    public InGameTextManager UITextCanvas;

    public float fadeTime = 0.5f;

    public bool delayMapUpdate = false;

    public EntitiesManager entitiesManager;

    public PauseMenu pauseMenu;

    public EndScreen endScreen;

    public Vector2Int mapPrefabsSize = new Vector2Int(20, 20);

    private float timePlayed;
    public bool countTimePlayed = true;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        GlobalData.maxTimemaps = mapSize;

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        try
        {
            foreach (var item in generatedMapsFoldersToHide)
            {
                item.SetActive(false);
            }
        }
        catch (Exception)
        {
            Debug.LogError("Them generatedMapsFolder is not assigned or smt");
        }        

        forceMoveArray = new Vector2[]
            { new Vector2(0, forceMoveDistance * 1.5f), new Vector2(forceMoveDistance, 0), new Vector2(0, -forceMoveDistance * 1.5f), new Vector2(-forceMoveDistance, 0) };
        if (isMinimapAvailable)
            minimap = GetComponent<MinimapController>();

        savingAndLoading = GameStateManager.instance.GetComponent<SavingAndLoading>();
        maxTilesInMap = GlobalData.maxTilemapsInMap;
        centerMapOffset = maxTilesInMap / 2;

        int mapNumber = spawnMapPos.x * mapPrefabsSize.x + spawnMapPos.y + 1;

        CreateZone(0, mapNumber);
    }

    public void CreateZone(int zoneNumber, int _spawnMapNumber)
    {

        spawnMapNumber = _spawnMapNumber;

#if (UNITY_EDITOR)

        if (GameObject.Find("MapGenerator") != null)
        {
            GameObject.Find("MapGenerator").GetComponent<TilemapGenerator>().SetupCompositeMaps();
        }
#endif

        if (FindObjectOfType<CompositeMapController>() != null)
        {
            FindObjectOfType<CompositeMapController>().SetupCompositeMaps();
        }

        preloadedMaps = new GameObject[(int)GlobalData.maxTimemaps, (int)GlobalData.maxTimemaps];

        if (isMinimapAvailable)
            minimap.SetupMinimap(maxTilesInMap, maxTilesInMap);
        LoadMapsToArray();

        Map currentMap = null;
        currentMap = LoadGame();
        CreateMap(currentMap, true);
        //if (!preloadMaps)
        //{
        //    currentMap = LoadGame();
        //    CreateMap(currentMap, true);
        //}
        //else
        //{
        //    PreloadAllMaps();
        //}

        ChangeAudioOnMapChange(currentMap.mapPrefab.GetComponent<CustomTilemap>().zone);
    }

    private void Update()
    {
        if (countTimePlayed)
            timePlayed += Time.deltaTime;

        float x = player.transform.position.x + centerMapOffset * GlobalData.maxTimemaps;
        float y = player.transform.position.y + centerMapOffset * GlobalData.maxTimemaps;

        int mapXPos = (int)((x + GlobalData.maxTimemaps / 2) / GlobalData.maxTimemaps);
        int mapYPos = (int)((y + GlobalData.maxTimemaps / 2) / GlobalData.maxTimemaps);

        if (!isMinimapAvailable)
            return;

        if (currentPos.x != mapXPos || currentPos.y != mapYPos)
        {
            if (currentMaps.Count > 1)
            {
                if (currentMaps[0].TryGetComponent(out CompositeTilemap component))
                {
                    minimap.UpdateUnlockedMinimap(mapXPos, mapYPos, mapArray[mapXPos, mapYPos], currentMaps);
                }
                else
                {
                    Debug.LogError("Cannot load map!");
                }
            }
            else
            {
                minimap.UpdateUnlockedMinimap(mapXPos, mapYPos, mapArray[mapXPos, mapYPos]);
            }
            currentPos.x = mapXPos;
            currentPos.y = mapYPos;
        }

    }

    private void LoadMapsToArray()
    {
        mapArray = new Map[maxTilesInMap, maxTilesInMap];
        int i = 0;
        foreach (Map map in currentMapList.maps)
        {
            if(map != null)
            {
                map.mapXOffset = (int)(map.mapPrefab.transform.position.x / GlobalData.maxTimemaps);
                map.mapYOffset = (int)(map.mapPrefab.transform.position.y / GlobalData.maxTimemaps);
                map.mapID = i;
                map.xSize = (int)GlobalData.maxTimemaps;
                map.ySize = (int)GlobalData.maxTimemaps;

                mapArray[centerMapOffset + map.mapXOffset, centerMapOffset + map.mapYOffset] = map;

                i++;
            }
        }
    }

    #region SavingAndLoading Methods

    private bool MT221()
    {
        string MT221T = Path.Combine(Application.persistentDataPath, $"MT221.txt");
        if (File.Exists(MT221T))
        {
            Debug.Log("Exist");
            return true;
        }
        else
        {
            spawnMapNumber = (1 * 20) + (1 + 1);
            return false;
        }
    }

    private Map LoadGame()
    {
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);

        player.customCollider.enabled = false;

        MT221();

        if (!savingAndLoading.CheckIfSaveFileExists(save))
        {
            if(spawnMapNumber - 1 > -1 && spawnMapNumber - 1 < currentMapList.maps.Count)
            {
                player.characterController.ForceTransportPlayer(new Vector2(currentMapList.maps[spawnMapNumber - 1].mapXOffset * GlobalData.maxTimemaps, currentMapList.maps[spawnMapNumber - 1].mapYOffset * GlobalData.maxTimemaps));

                player.customCollider.enabled = true;
                return mapArray[centerMapOffset + currentMapList.maps[spawnMapNumber - 1].mapXOffset, centerMapOffset + currentMapList.maps[spawnMapNumber - 1].mapYOffset];
            }

            Debug.LogWarning($"Index out of bounds! spawnMapNumber is set to {spawnMapNumber - 1}. Setting spawn to {centerMapOffset}, {centerMapOffset}");
            player.customCollider.enabled = true;
            return mapArray[centerMapOffset, centerMapOffset];
        }
        if (!willLoadSaveFile)
        {
            player.characterController.ForceTransportPlayer(new Vector2(currentMapList.maps[spawnMapNumber - 1].mapXOffset * GlobalData.maxTimemaps, currentMapList.maps[spawnMapNumber - 1].mapYOffset * GlobalData.maxTimemaps));

            player.customCollider.enabled = true;

            return mapArray[centerMapOffset + currentMapList.maps[spawnMapNumber - 1].mapXOffset, centerMapOffset + currentMapList.maps[spawnMapNumber - 1].mapYOffset];
        }

        savingAndLoading.LoadGameFile(save);
        player.progressTracker.bossesSlayed = save.bossesSlayed;
        player.progressTracker.collectibles = save.collectibles;

        timePlayed = save.timePlayed;

        player.playerData.maxHealth = save.maxHealth;
        player.playerData.health = save.currentHealth;
        player.playerData.maxArrowCount = save.maxArrowCount;
        player.playerData.currentArrowCount = save.maxArrowCount;

        player.playerData.WaterSpirit = save.waterSpirit;
        player.playerData.EarthSpirit = save.earthSpirit;
        player.playerData.FireSpirit = save.fireSpirit;
        player.playerWeaponSwap.unlockedWeapons = save.unlockedWeapons;

        player.playerData.damageModifier = save.damageModifier;
        player.playerData.reloadSpeedModifier = save.reloadSpeedModifier;

        player.OnBowUnlocked(save.unlockedBow);

        player.RefreshUI();

        minimap.LoadMinimap(save);
        minimap.UpdateMinimap(save.mapXOffset + centerMapOffset, save.mapYOffset + centerMapOffset);

        player.characterController.ForceTransportPlayer(new Vector2(save.mapXOffset * GlobalData.maxTimemaps, save.mapYOffset * GlobalData.maxTimemaps));

        player.customCollider.enabled = true;


        return mapArray[save.mapXOffset + centerMapOffset, save.mapYOffset + centerMapOffset]; //force player to save at [1,1] tile maps

    }


    public void SaveGame()
    {
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);

        if (savingAndLoading.CheckIfSaveFileExists(save))
        {
            
        }

        minimap.SaveMinimap(save);
        save.mapXOffset = mapArray[(int)currentPos.x, (int)currentPos.y].mapXOffset;
        save.mapYOffset = mapArray[(int)currentPos.x, (int)currentPos.y].mapYOffset;
        save.mapID = mapArray[(int)currentPos.x, (int)currentPos.y].mapID;

        save.zone = mapArray[(int)currentPos.x, (int)currentPos.y].mapPrefab.GetComponent<CustomTilemap>().zone;
        save.timePlayed = timePlayed;

        save.bossesSlayed = player.progressTracker.bossesSlayed;
        save.collectibles = player.progressTracker.collectibles;

        save.maxHealth = player.playerData.maxHealth;
        save.currentHealth = player.playerData.health;
        save.maxArrowCount = player.playerData.maxArrowCount;
        save.currentArrowCount = player.playerData.currentArrowCount;
        save.unlockedWeapons = player.playerWeaponSwap.unlockedWeapons;

        save.unlockedBow = player.playerData.unlockedBow;
        save.waterSpirit = player.playerData.WaterSpirit;
        save.earthSpirit = player.playerData.EarthSpirit;
        save.fireSpirit = player.playerData.FireSpirit;

        save.damageModifier = player.playerData.damageModifier;
        save.reloadSpeedModifier = player.playerData.reloadSpeedModifier;

        savingAndLoading.SaveGameFile(save);
    }

    #endregion

    private void CreateMap(Map currentMap, bool enableTriggers)
    {
        bool isRoomABossRoom = false;

        currentMaps.Add(Instantiate(currentMap.mapPrefab));

        //AstarPath.active.data.gridGraph.center = new Vector3(currentMap.mapXOffset * GlobalData.maxTilesInMap, currentMap.mapYOffset * GlobalData.maxTilesInMap, 1f);
        //AstarPath.active.Scan();

        currentMaps[0].GetComponent<CustomTilemap>().map = currentMap;
        isRoomABossRoom = currentMaps[0].GetComponent<CustomTilemap>().OnRoomEnter();
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
                    if(!isRoomABossRoom)
                        isRoomABossRoom = currentMaps[i].GetComponent<CustomTilemap>().OnRoomEnter();
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

                    foreach (var map in tilemap.map.mapDoors)
                    {
                        if (mapBorderCollision.direction == map)
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

                        //CameraMovement camera = Camera.main.GetComponent<CameraMovement>();

                        if (mapBorderCollision.TryGetComponent(out ChangeZoneScript changeZoneScript))
                        {
                            ChangeTilemapOnBorderWithZoneScriptEntry(changeZoneScript, mapBorderCollision, connectedMapXOffset, connectedMapYOffset, tilemap);
                        }
                        else
                        {
                            ChangeTilemapOnBorderEntry(mapBorderCollision, connectedMapXOffset, connectedMapYOffset, tilemap);
                        }

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

    private void ChangeTilemapOnBorderEntry(MapBorderCollision mapBorderCollision, int connectedMapXOffset, int connectedMapYOffset, CustomTilemap tilemap)
    {
        CameraMovement camera = Camera.main.GetComponent<CameraMovement>();

        //Debug.Log(connectedMapXOffset + " " + connectedMapYOffset);
        //Debug.Log(tilemap.map.mapXOffset + " " + tilemap.map.mapYOffset);
        mapBorderCollision.onTriggerEnterEvent.AddListener(delegate
        {
            player.characterController.StopMovement(true);
            camera.blackout.DOFade(1, fadeTime).OnComplete(() =>
            {
                if (!camera.stopCamera)
                    camera.SnapCameraPosition();
                ChangeCurrentMap(mapArray[connectedMapXOffset, connectedMapYOffset]);
                ChangeAudioOnMapChange(mapArray[connectedMapXOffset, connectedMapYOffset].mapPrefab.GetComponent<CustomTilemap>().zone);
                camera.blackout.DOFade(0, fadeTime);
                player.characterController.ForceTransportPlayer(forceMoveArray[(int)mapBorderCollision.direction] + mapBorderCollision.additionalPlayerMove);
                player.characterController.StopMovement(false);
                tilemap.EnableAllTriggers();
            });
        });
    }

    private void ChangeTilemapOnBorderWithZoneScriptEntry(ChangeZoneScript changeZoneScript, MapBorderCollision mapBorderCollision, int connectedMapXOffset, int connectedMapYOffset, CustomTilemap tilemap)
    {
        CameraMovement camera = Camera.main.GetComponent<CameraMovement>();

        if (changeZoneScript.IsMapPreloaded)
        {
            mapBorderCollision.onTriggerEnterEvent.AddListener(delegate
            {
                connectedMapXOffset = centerMapOffset + changeZoneScript.nextZonePos.x;
                connectedMapYOffset = centerMapOffset + changeZoneScript.nextZonePos.y;

                player.characterController.StopMovement(true);
                camera.blackout.DOFade(1, fadeTime).OnComplete(() =>
                {
                    if (!camera.stopCamera)
                        camera.SnapCameraPosition();
                    ChangeAudioOnMapChange(mapArray[connectedMapXOffset, connectedMapYOffset].mapPrefab.GetComponent<CustomTilemap>().zone);
                    camera.blackout.DOFade(0, fadeTime);

                    Vector2[] forceMoveMagnitudeArray = new Vector2[]
                        { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

                    int nextZoneXDifference = changeZoneScript.nextZonePos.x - tilemap.map.mapXOffset;
                    int nextZoneYDifference = changeZoneScript.nextZonePos.y - tilemap.map.mapYOffset;

                    float tempX = (nextZoneXDifference * GlobalData.maxTimemaps) + forceMoveArray[(int)mapBorderCollision.direction].x - (forceMoveMagnitudeArray[(int)mapBorderCollision.direction].x * GlobalData.maxTimemaps);
                    float tempY = (nextZoneYDifference * GlobalData.maxTimemaps) + forceMoveArray[(int)mapBorderCollision.direction].y - (forceMoveMagnitudeArray[(int)mapBorderCollision.direction].y * GlobalData.maxTimemaps);

                    player.characterController.ForceTransportPlayer(new Vector2(tempX, tempY) + mapBorderCollision.additionalPlayerMove);
                    player.characterController.StopMovement(false);
                    tilemap.EnableAllTriggers();
                });
            });
        }
        else
        {
            mapBorderCollision.onTriggerEnterEvent.AddListener(delegate
            {
                connectedMapXOffset = centerMapOffset + changeZoneScript.nextZonePos.x;
                connectedMapYOffset = centerMapOffset + changeZoneScript.nextZonePos.y;

                player.characterController.StopMovement(true);
                camera.blackout.DOFade(1, fadeTime).OnComplete(() =>
                {
                    if (!camera.stopCamera)
                        camera.SnapCameraPosition();
                    ChangeCurrentMap(mapArray[connectedMapXOffset, connectedMapYOffset]);
                    ChangeAudioOnMapChange(mapArray[connectedMapXOffset, connectedMapYOffset].mapPrefab.GetComponent<CustomTilemap>().zone);
                    camera.blackout.DOFade(0, fadeTime);

                    Vector2[] forceMoveMagnitudeArray = new Vector2[]
                        { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };


                    int nextZoneXDifference = changeZoneScript.nextZonePos.x - tilemap.map.mapXOffset;
                    int nextZoneYDifference = changeZoneScript.nextZonePos.y - tilemap.map.mapYOffset;

                    float tempX = (nextZoneXDifference * GlobalData.maxTimemaps) + forceMoveArray[(int)mapBorderCollision.direction].x - (forceMoveMagnitudeArray[(int)mapBorderCollision.direction].x * GlobalData.maxTimemaps);
                    float tempY = (nextZoneYDifference * GlobalData.maxTimemaps) + forceMoveArray[(int)mapBorderCollision.direction].y - (forceMoveMagnitudeArray[(int)mapBorderCollision.direction].y * GlobalData.maxTimemaps);




                    player.characterController.ForceTransportPlayer(new Vector2(tempX, tempY));
                    //player.characterController.ForceTransportPlayerToPosition(new Vector2(tempX, tempY));
                    player.characterController.StopMovement(false);
                    tilemap.EnableAllTriggers();
                });
            });
        }
    }

    private void PreloadAllMaps()
    {
        int i = 0;
        foreach (var map in mapArray)
        {
            if(map == null)
                continue;
            
            currentMaps.Add(Instantiate(map.mapPrefab));

            CustomTilemap tilemap = currentMaps[i].GetComponent<CustomTilemap>();
            tilemap.map = map;
            tilemap.OnRoomEnter();
            tilemap.EnableAllTriggers();

            xSizeMul = 1;
            ySizeMul = 1;

            if (currentMaps[i].TryGetComponent(out CompositeTilemap component))
            {
                xSizeMul = component.compMapXSize;
                ySizeMul = component.compMapYSize;
            }

            foreach (Transform childCollider in tilemap.outsideColliders.transform)
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

                    if (mapBorderCollision.isADoor)
                    {
                        int connectedMapXOffset = centerMapOffset + tilemap.map.mapXOffset + directionArray[(int)mapBorderCollision.direction, 0];
                        int connectedMapYOffset = centerMapOffset + tilemap.map.mapYOffset + directionArray[(int)mapBorderCollision.direction, 1];

                        mapBorderCollision.onTriggerEnterEvent.AddListener(delegate
                        {
                            ChangePreloadedMap(mapArray[connectedMapXOffset, connectedMapYOffset]);
                            player.characterController.ForceMovePlayer(forceMoveArray[(int)mapBorderCollision.direction]);
                            Camera.main.GetComponent<CameraMovement>().MoveCameraBetweenZones(forceMoveArray[(int)mapBorderCollision.direction]);
                            GameStateManager.instance.audioManager.OnMapChange(tilemap.zone);
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

            currentMaps[i].transform.SetParent(preloadedMapsParent);
            preloadedMaps[map.mapXOffset, map.mapYOffset] = currentMaps[i];
            i++;
        }

    }
    private void SetCameraBoundaries()
    {
        float xCoord = 0;
        float yCoord = 0;

        foreach (GameObject mapOnScene in currentMaps)
        {
            xCoord = xCoord + (mapOnScene.GetComponent<CustomTilemap>().map.mapXOffset * GlobalData.maxTimemaps);
            yCoord = yCoord + (mapOnScene.GetComponent<CustomTilemap>().map.mapYOffset * GlobalData.maxTimemaps);
        }

        xCoord = NumericExtensions.SafeDivision(xCoord, xSizeMul * ySizeMul);
        yCoord = NumericExtensions.SafeDivision(yCoord, xSizeMul * ySizeMul);

        CameraMovement camera = Camera.main.GetComponent<CameraMovement>();

        camera.SetCameraCenter(new Vector2(xCoord, yCoord));
        if(!camera.stopCamera)
            camera.MoveCamera(new Vector2(xCoord, yCoord));
        camera.SetCameraBoundary(xSizeMul, ySizeMul);
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
    public void ChangePreloadedMap(Map nextMap)
    {
        foreach (GameObject map in preloadedMaps)
        {
            if(map == null)
                continue;
            map.SetActive(false);
        }
        preloadedMaps[nextMap.mapXOffset, nextMap.mapYOffset].GetComponent<CustomTilemap>().DisableAllTriggers(1);
        preloadedMaps[nextMap.mapXOffset, nextMap.mapYOffset].SetActive(true);
    }
    private void ChangeAudioOnMapChange(Zone zone)
    {
        GameStateManager.instance.audioManager.OnMapChange(zone);
    }

    public void LoadGameButton()
    {
        //Load Game by reloading the scene
        GameStateManager.instance.LoadGameSceneWithLoadingScreen();

        //Load Game in the same scene;
        //Map map = LoadGame();
        //ChangeCurrentMap(map);
        //ChangeAudioOnMapChange(map.mapPrefab.GetComponent<CustomTilemap>().zone);
    }
}


static public class NumericExtensions
{
    static public float SafeDivision(float Numerator, float Denominator)
    {
        return (Denominator == 0) ? 0 : Numerator / Denominator;
    }
}


