using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMap : CustomUIMenu
{
    [SerializeField]
    private List<MinimapSprite> minimapTiles;

    private Image[,] backgroundSpritesArray;
    private Image[,] wallSpritesminimapArray;
    private Image[,] doorSpritesminimapArray;
    private Image[,] specialSpritesminimapArray;

    [SerializeField]
    private Sprite emptyMinimapTile;

    private int minimapXSize;
    private int minimapYSize;

    [SerializeField]
    private GameObject currentTileHighlight;
    [SerializeField]
    private GameObject currentTileHighlight2;
    [SerializeField]
    private Transform mapHolder;

    [SerializeField]
    private float tempXPos;
    [SerializeField]
    private float tempYPos;

    private void Awake()
    {
        SetupMinimap(GlobalData.maxTilemapsInMap/2, GlobalData.maxTilemapsInMap/2);
    }

    public override void OnMenuSwap()
    {
        SetMinimap();
        currentTileHighlight.GetComponent<MinimapAnimation>().RestartCoroutine();
        currentTileHighlight2.GetComponent<MinimapAnimation>().RestartCoroutine();
    }

    public void SetupMinimap(int _minimapXSize, int _minimapYSize)
    {
        minimapXSize = _minimapXSize;
        minimapYSize = _minimapYSize;


        backgroundSpritesArray = new Image[minimapXSize, minimapYSize];
        wallSpritesminimapArray = new Image[minimapXSize, minimapYSize];
        doorSpritesminimapArray = new Image[minimapXSize, minimapYSize];
        specialSpritesminimapArray = new Image[minimapXSize, minimapYSize];

        for (int i = 0; i < minimapXSize; i++)
        {
            for (int j = 0; j < minimapYSize; j++)
            {
                backgroundSpritesArray[i, j] = minimapTiles[i + j * minimapYSize].background;
                wallSpritesminimapArray[i, j] = minimapTiles[i + j * minimapYSize].walls;
                doorSpritesminimapArray[i, j] = minimapTiles[i + j * minimapYSize].doors;
                specialSpritesminimapArray[i, j] = minimapTiles[i + j * minimapYSize].special;

                backgroundSpritesArray[i, j].enabled = true;
                wallSpritesminimapArray[i, j].enabled = false;
                doorSpritesminimapArray[i, j].enabled = false;
                specialSpritesminimapArray[i, j].enabled = false;
            }
        }
    }

    public void ClearMinimap()
    {
        foreach (MinimapSprite minimapSprite in minimapTiles)
        {
            minimapSprite.background.sprite = emptyMinimapTile;
            minimapSprite.walls.enabled = false;
            minimapSprite.doors.enabled = false;
            minimapSprite.special.enabled = false;
        }
    }

    public void SetMinimap()
    {
        MapIcon[,] mapIcons = GameManagerScript.instance.minimap.GetUnlockedMap();
        //bool[,] unlockedMaps = GameManagerScript.instance.minimap.GetUnlockedMapBool();
        //Sprite[,] unlockedBackground = GameManagerScript.instance.minimap.GetUnlockedBackground();
        //Sprite[,] unlockedWalls = GameManagerScript.instance.minimap.GetUnlockedWalls();
        //Sprite[,] unlockedDoors = GameManagerScript.instance.minimap.GetUnlockedDoors();

        ClearMinimap();
        for (int i = 0; i < minimapXSize; i++)
        {
            for (int j = 0; j < minimapYSize; j++)
            {
                //Debug.Log(unlockedMaps[i, j]);
                if (mapIcons[i, j].isUnlocked)
                {
                    wallSpritesminimapArray[i, j].enabled = true;
                    doorSpritesminimapArray[i, j].enabled = true;
                    specialSpritesminimapArray[i, j].enabled = true;

                    backgroundSpritesArray[i, j].sprite = mapIcons[i, j].background;
                    wallSpritesminimapArray[i, j].sprite = mapIcons[i, j].walls;
                    doorSpritesminimapArray[i, j].sprite = mapIcons[i, j].doors;
                    specialSpritesminimapArray[i, j].sprite = mapIcons[i, j].specials;
                }
            }
        }

        Vector2Int currentPos = GameManagerScript.instance.minimap.GetCurrentLocation();
        //Debug.Log(currentPos.x + " " + currentPos.y);

        float mapTileSize = 16;

        float temp1 = -((currentPos.x - 32) * mapTileSize);
        float temp2 = -((currentPos.y - 32) * mapTileSize);

        mapHolder.transform.localPosition = new Vector3(temp1, temp2, 0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xDirection">1 - left, -1 - right</param>
    /// <param name="yDirection">-1 - down, 1 - up</param>
    public void MoveMap(int xDirection, int yDirection)
    {
        float mapTileSize = 16;
        Vector2 currentPos = mapHolder.transform.localPosition;
        float temp1 = currentPos.x + xDirection * mapTileSize;
        float temp2 = currentPos.y + yDirection * mapTileSize;

        mapHolder.transform.localPosition = new Vector3(temp1, temp2, 0f);
    }
}
