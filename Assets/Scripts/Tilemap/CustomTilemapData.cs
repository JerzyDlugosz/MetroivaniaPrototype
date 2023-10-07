using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTilemapData : MonoBehaviour
{
    public Tilemap BackgroundTilemap;
    public Tilemap WallsTilemap;
    public Tilemap ForegroundTilemap;

    public bool wasEdited;
    public bool isPlayable;

    public int xPos;
    public int yPos;
}
