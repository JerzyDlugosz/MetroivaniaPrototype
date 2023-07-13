using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private List<MinimapSprite> minimapTiles;

    private Image[,] backgroundSpritesArray;
    private Image[,] wallSpritesminimapArray;
    private Image[,] doorSpritesminimapArray;


    [SerializeField]
    private Sprite emptyMinimapTile;

    private int minimapXSize;
    private int minimapYSize;

    public void SetupMinimap(Sprite backgroundSprite, int _minimapXSize, int _minimapYSize)
    {
        minimapXSize = _minimapXSize;
        minimapYSize = _minimapYSize;


        backgroundSpritesArray = new Image[minimapXSize, minimapYSize];
        wallSpritesminimapArray = new Image[minimapXSize, minimapYSize];
        doorSpritesminimapArray = new Image[minimapXSize, minimapYSize];

        for (int i = 0; i < minimapXSize; i++)
        {
            for (int j = 0; j < minimapYSize; j++)
            {
                backgroundSpritesArray[i, j] = minimapTiles[i + j * minimapYSize].background;
                wallSpritesminimapArray[i, j] = minimapTiles[i + j * minimapYSize].walls;
                doorSpritesminimapArray[i, j] = minimapTiles[i + j * minimapYSize].doors;

                backgroundSpritesArray[i, j].sprite = backgroundSprite;

                backgroundSpritesArray[i, j].enabled = true;
                wallSpritesminimapArray[i, j].enabled = true;
                doorSpritesminimapArray[i, j].enabled = true;
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
        }
    }

    public void SetMinimap(bool[,] unlockedMinimap, List<Sprite[,]> unlockedMinimapSprites)
    {
        ClearMinimap();
        for (int i = 0; i < minimapXSize; i++)
        {
            for (int j = 0; j < minimapYSize; j++)
            {
                if (unlockedMinimap[i,j])
                {
                    wallSpritesminimapArray[i, j].enabled = true;
                    doorSpritesminimapArray[i, j].enabled = true;

                    backgroundSpritesArray[i, j].sprite = unlockedMinimapSprites[0][i, j];
                    wallSpritesminimapArray[i, j].sprite = unlockedMinimapSprites[1][i, j];
                    doorSpritesminimapArray[i, j].sprite = unlockedMinimapSprites[2][i, j];
                    //wallSpritesminimapArray[i, j].enabled = unlockedMinimap[i, j];
                }
            }
        }
    }
}
