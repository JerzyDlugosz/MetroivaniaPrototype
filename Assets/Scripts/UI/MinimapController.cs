using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private bool[,] unlockedMap;
    private bool[,] currentMapMinimap;
    private int currentXPos;
    private int currentYPos;

    [SerializeField]
    private Minimap minimap;

    [SerializeField]
    private List<Sprite> backgroundSprites;
    [SerializeField]
    private List<Sprite> wallSprites;
    [SerializeField]
    private List<Sprite> doorSprites;
    [SerializeField]
    private List<Sprite> specialSprites; //0-NormalRoom, 1-SaveRoom, 2-BossRoom, 3-???

    private Sprite[,] unlockedMinimapBackgroundSprites;
    private Sprite[,] unlockedMinimapWallSprites;
    private Sprite[,] unlockedMinimaDoorSprites;

    private int[,] unlockedMinimapBackgrounds;
    private int[,] unlockedMinimapWalls;
    private int[,] unlockedMinimapDoors;
    private int[,] unlockedSpecialRooms;


    [SerializeField]
    private int minimapXSize;
    [SerializeField]
    private int minimapYSize;

    private int maxTilesInMap;

    private MapIcon[,] mapIcons;

    public void MinimapState(bool state)
    {
        minimap.gameObject.SetActive(state);
    }


    public void SetupMinimap(int xSize, int ySize)
    {

        maxTilesInMap = GlobalData.maxTilemapsInMap;

        //unlockedMinimapBackgroundSprites = new Sprite[xSize, ySize];
        //unlockedMinimapWallSprites = new Sprite[xSize, ySize];
        //unlockedMinimaDoorSprites = new Sprite[xSize, ySize];

        unlockedMinimapBackgrounds = new int[xSize, ySize];
        unlockedMinimapWalls = new int[xSize, ySize];
        unlockedMinimapDoors = new int[xSize, ySize];
        unlockedSpecialRooms = new int[xSize, ySize];

        unlockedMap = new bool[xSize, ySize];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                unlockedMap[i, j] = false;
            }
        }

        currentMapMinimap = new bool[xSize, ySize];
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                currentMapMinimap[i, j] = false;
                currentXPos = i;
                currentYPos = j;
            }
        }
        //Load stuff from savefile
        minimap.SetupMinimap(backgroundSprites[0], minimapXSize, minimapYSize);
    }

    public void UpdateUnlockedMinimap(int mapXPos, int mapYPos, Map currentMap)
    {
        if (!unlockedMap[mapXPos,mapYPos])
        {
            unlockedMap[mapXPos, mapYPos] = true;
        }

        if(currentXPos != mapXPos || currentYPos != mapYPos)
        {
            currentMapMinimap[currentXPos, currentYPos] = false;
            currentMapMinimap[mapXPos, mapYPos] = true;

            int doorBitDirection = 0;
            foreach (var item in currentMap.mapDoors)
            {
                if (item == Direction.Right)
                {
                    doorBitDirection += 1;
                }
                if (item == Direction.Up)
                {
                    doorBitDirection += 2;
                }
                if (item == Direction.Left)
                {
                    doorBitDirection += 4;
                }
                if (item == Direction.Down)
                {
                    doorBitDirection += 8;
                }
            }

            unlockedMinimapBackgrounds[mapXPos, mapYPos] = (int)currentMap.mapPrefab.GetComponent<CustomTilemap>().zone;
            unlockedMinimapWalls[mapXPos, mapYPos] = 0;
            unlockedMinimapDoors[mapXPos, mapYPos] = doorBitDirection;
            unlockedSpecialRooms[mapXPos, mapYPos] = (int)currentMap.mapPrefab.GetComponent<CustomTilemap>().specialRoom;

            //unlockedMinimapBackgroundSprites[mapXPos, mapYPos] = backgroundSprites[0];
            //unlockedMinimapWallSprites[mapXPos, mapYPos] = wallSprites[0];
            //unlockedMinimaDoorSprites[mapXPos, mapYPos] = doorSprites[doorBitDirection];


            currentXPos = mapXPos;
            currentYPos = mapYPos;

            UpdateMinimap(mapXPos, mapYPos);
        }
    }

    public void ManuallySetMinimapTile(int mapXPos, int mapYPos, Map currentMap, int specialIcon)
    {
        if (!unlockedMap[mapXPos, mapYPos])
        {
            unlockedMap[mapXPos, mapYPos] = true;
        }

        if (currentXPos != mapXPos || currentYPos != mapYPos)
        {
            currentMapMinimap[currentXPos, currentYPos] = false;
            currentMapMinimap[mapXPos, mapYPos] = true;

            int doorBitDirection = 0;
            foreach (var item in currentMap.mapDoors)
            {
                if (item == Direction.Right)
                {
                    doorBitDirection += 1;
                }
                if (item == Direction.Up)
                {
                    doorBitDirection += 2;
                }
                if (item == Direction.Left)
                {
                    doorBitDirection += 4;
                }
                if (item == Direction.Down)
                {
                    doorBitDirection += 8;
                }
            }

            unlockedMinimapBackgrounds[mapXPos, mapYPos] = (int)currentMap.mapPrefab.GetComponent<CustomTilemap>().zone;
            unlockedMinimapWalls[mapXPos, mapYPos] = 0;
            unlockedMinimapDoors[mapXPos, mapYPos] = doorBitDirection;
            unlockedSpecialRooms[mapXPos, mapYPos] = specialIcon;

            //unlockedMinimapBackgroundSprites[mapXPos, mapYPos] = backgroundSprites[0];
            //unlockedMinimapWallSprites[mapXPos, mapYPos] = wallSprites[0];
            //unlockedMinimaDoorSprites[mapXPos, mapYPos] = doorSprites[doorBitDirection];


            currentXPos = mapXPos;
            currentYPos = mapYPos;

            //UpdateMinimap(mapXPos, mapYPos);
        }
    }

    public void UpdateUnlockedMinimap(int mapXPos, int mapYPos, Map currentMap, List<GameObject> compMaps)
    {
        if (!unlockedMap[mapXPos, mapYPos])
        {
            unlockedMap[mapXPos, mapYPos] = true;
        }

        if (currentXPos != mapXPos || currentYPos != mapYPos)
        {
            currentMapMinimap[currentXPos, currentYPos] = false;
            currentMapMinimap[mapXPos, mapYPos] = true;

            int doorBitDirection = 0;
            foreach (var item in currentMap.mapDoors)
            {
                if (item == Direction.Right)
                {
                    doorBitDirection += 1;
                }
                if (item == Direction.Up)
                {
                    doorBitDirection += 2;
                }
                if (item == Direction.Left)
                {
                    doorBitDirection += 4;
                }
                if (item == Direction.Down)
                {
                    doorBitDirection += 8;
                }
            }


            int wallBitDirection = 0;
            int i = 0;
            foreach (var item in compMaps)
            {
                if(!item.TryGetComponent(out CustomTilemap tilemap))
                {
                    Debug.LogError("No tilemap script in one of tilemap in compositeTilemaps group");
                }

                int x = currentMap.mapXOffset * -1 - tilemap.map.mapXOffset * -1;     //Abs does not work for example [5,1],[5,0],[5,-1]
                int y = currentMap.mapYOffset * -1 - tilemap.map.mapYOffset * -1;     //Need to test it on positive/ positive and negative numbers

                //Debug.Log($"{currentMap.name} and {item.name}");

                //Debug.Log($"{currentMap.mapXOffset},{currentMap.mapYOffset} i {tilemap.map.mapXOffset},{tilemap.map.mapYOffset}");
                if (y == 0)
                {
                    if (x == 1)
                    {
                        wallBitDirection += 1;
                    }
                    else if (x == -1)
                    {
                        wallBitDirection += 4;
                    }
                }
                else if(x == 0)
                {
                    if (y == 1)
                    {
                        wallBitDirection += 2;
                    }
                    else if (y == -1)
                    {
                        wallBitDirection += 8;
                    }
                }
                //Debug.Log(wallBitDirection);
                i++;
            }



            unlockedMinimapBackgrounds[mapXPos, mapYPos] = (int)currentMap.mapPrefab.GetComponent<CustomTilemap>().zone;
            unlockedMinimapWalls[mapXPos, mapYPos] = wallBitDirection;
            unlockedMinimapDoors[mapXPos, mapYPos] = doorBitDirection;
            unlockedSpecialRooms[mapXPos, mapYPos] = (int)currentMap.mapPrefab.GetComponent<CustomTilemap>().specialRoom;

            //unlockedMinimapBackgroundSprites[mapXPos, mapYPos] = backgroundSprites[0];
            //unlockedMinimapWallSprites[mapXPos, mapYPos] = wallSprites[wallBitDirection];
            //unlockedMinimaDoorSprites[mapXPos, mapYPos] = doorSprites[doorBitDirection];

            currentXPos = mapXPos;
            currentYPos = mapYPos;


            UpdateMinimap(mapXPos, mapYPos);
        }
    }

    public void UpdateMinimap(int xPosOffset, int yPosOffset)
    {
        SetupMapIcons(unlockedMap, unlockedMinimapBackgrounds, unlockedMinimapWalls, unlockedMinimapDoors, unlockedSpecialRooms);



        //Change the whole 100,100 arrays into small 5,5 or 6,6 arrays for minimap
        var unlockedMinimap = new bool[minimapXSize + 1, minimapYSize + 1];
        var MinimapBcgSprites = new Sprite[minimapXSize + 1, minimapYSize + 1];
        var MinimapWallSprites = new Sprite[minimapXSize + 1, minimapYSize + 1];
        var MinimapdoorSprites = new Sprite[minimapXSize + 1, minimapYSize + 1];
        var SpecialRoomSprites = new Sprite[minimapXSize + 1, minimapYSize + 1];
        int x = 0;
        int y = 0;



        for (int i = xPosOffset - (int)(minimapXSize / 2); i < xPosOffset + (int)(minimapXSize / 2) + 1; i++)
        {
            y = 0;
            for (int j = yPosOffset - (int)(minimapYSize / 2); j < yPosOffset + (int)(minimapYSize / 2) + 1; j++)
            {

                unlockedMinimap[x, y] = unlockedMap[i, j];
                MinimapBcgSprites[x, y] = backgroundSprites[unlockedMinimapBackgrounds[i, j]];
                MinimapWallSprites[x, y] = wallSprites[unlockedMinimapWalls[i, j]];
                MinimapdoorSprites[x, y] = doorSprites[unlockedMinimapDoors[i, j]];
                SpecialRoomSprites[x, y] = specialSprites[unlockedSpecialRooms[i, j]];
                y++;
            }

            x++;
        }

        minimap.SetMinimap(unlockedMinimap, new List<Sprite[,]> { MinimapBcgSprites, MinimapWallSprites, MinimapdoorSprites, SpecialRoomSprites });
    }

    public void LoadMinimap(Save save)
    {

        for (int i = 0; i < maxTilesInMap; i++)
        {
            for (int j = 0; j < maxTilesInMap; j++)
            {
                unlockedMap[i, j] = save.unlockedMap[i * maxTilesInMap + j];
                unlockedMinimapBackgrounds[i, j] = save.unlockedMinimapBackgrounds[i * maxTilesInMap + j];
                unlockedMinimapDoors[i, j] = save.unlockedMinimapDoors[i * maxTilesInMap + j];
                unlockedMinimapWalls[i, j] = save.unlockedMinimapWalls[i * maxTilesInMap + j];
                unlockedSpecialRooms[i, j] = save.unlockedSpecialRooms[i * maxTilesInMap + j];


                //Debug.Log($"L1. {save.unlockedMap[i * maxTilesInMap + j]}, {save.unlockedMinimapBackgrounds[i * maxTilesInMap + j]}, {save.unlockedMinimapDoors[i * maxTilesInMap + j]}, {save.unlockedMinimapWalls[i * maxTilesInMap + j]} ");

                //Debug.Log($"L2. {unlockedMap[i, j]}, {unlockedMinimapBackgrounds[i, j]}, {unlockedMinimapDoors[i, j]}, {unlockedMinimapWalls[i, j]} ");
            }
        }

        //Make this into correct offset (save a map class for current map)

        //UpdateMinimap(maxTilesInMap/2, maxTilesInMap/2, unlockedMinimapBackgrounds, unlockedMinimapWalls, unlockedMinimaDoors);


        //unlockedMap = save.unlockedMap.ToArray();
        //unlockedMinimapBackgrounds = save.unlockedMinimapBackgrounds;
        //unlockedMinimaDoors = save.unlockedMinimaDoors;
        //unlockedMinimapWalls = save.unlockedMinimapWalls;


    }

    public void SaveMinimap(Save save)
    {

        //This does not work, fix it m8

        //Save save = GameStateManager.instance.GetComponent<SavingAndLoading>().GetSaveFile(GameStateManager.instance.GetComponent<SavingAndLoading>().currentSaveFile);

        //for (int i = 0; i < 100; i++)
        //{
        //    save.unlockedMap = new NestedBoolArray[100];
        //    save.unlockedMinimapBackgrounds = new NestedIntArray[100];
        //    save.unlockedMinimaDoors = new NestedIntArray[100];
        //    save.unlockedMinimapWalls = new NestedIntArray[100];
        //}


        for (int i = 0; i < maxTilesInMap; i++)
        {
            for (int j = 0; j < maxTilesInMap; j++)
            {
                save.unlockedMap[i * maxTilesInMap + j] = unlockedMap[i, j];
                save.unlockedMinimapBackgrounds[i * maxTilesInMap + j] = unlockedMinimapBackgrounds[i, j];
                save.unlockedMinimapDoors[i * maxTilesInMap + j] = unlockedMinimapDoors[i, j];
                save.unlockedMinimapWalls[i * maxTilesInMap + j] = unlockedMinimapWalls[i, j];
                save.unlockedSpecialRooms[i * maxTilesInMap + j] = unlockedSpecialRooms[i, j];

                //Debug.Log($"S1. {save.unlockedMap[i * maxTilesInMap + j]}, {save.unlockedMinimapBackgrounds[i * maxTilesInMap + j]}, {save.unlockedMinimapDoors[i * maxTilesInMap + j]}, {save.unlockedMinimapWalls[i * maxTilesInMap + j]} ");

                //Debug.Log($"S2. {unlockedMap[i, j]}, {unlockedMinimapBackgrounds[i, j]}, {unlockedMinimapDoors[i, j]}, {unlockedMinimapWalls[i, j]} ");

            }
        }

        //save.unlockedMap = unlockedMap;
        //save.unlockedMinimapBackgrounds = unlockedMinimapBackgrounds;
        //save.unlockedMinimaDoors = unlockedMinimaDoors;
        //save.unlockedMinimapWalls = unlockedMinimapWalls;
    }

    private void SetupMapIcons(bool[,] unlockedMaps, int[,] background, int[,] walls, int[,] doors, int[,] special)
    {
        int mapOffset = 20;
        int mapSize = GlobalData.maxTilemapsInMap / 2;
        mapIcons = new MapIcon[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                mapIcons[i, j] = new MapIcon(unlockedMaps[i + mapOffset, j + mapOffset], backgroundSprites[background[i + mapOffset, j + mapOffset]], wallSprites[walls[i + mapOffset, j + mapOffset]], doorSprites[doors[i + mapOffset, j + mapOffset]], specialSprites[special[i + mapOffset, j + mapOffset]]);
            }
        }
    }

    public MapIcon[,] GetUnlockedMap()
    {
        return mapIcons;
    }

    public Vector2Int GetCurrentLocation()
    {
        Vector2Int currentPos = new Vector2Int(currentXPos, currentYPos);
        return currentPos;
    }
}

public class MapIcon
{
    public MapIcon(bool _isUnlocked, Sprite _background, Sprite _walls, Sprite _doors, Sprite _specials)
    {
        isUnlocked = _isUnlocked;
        background = _background;
        walls = _walls;
        doors = _doors;
        specials = _specials;
    }

    public bool isUnlocked;
    public Sprite background;
    public Sprite walls;
    public Sprite doors;
    public Sprite specials;
}
