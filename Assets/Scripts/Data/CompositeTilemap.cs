using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeTilemap : MonoBehaviour
{
    public List<Map> connectedTilemaps;
    public int compMapXSize;
    public int compMapYSize;

    /// <summary>
    /// Used in tilemapGenerator. Specifies the x range of the composite map (Min - Max)
    /// </summary>
    public Vector2Int compMapXPos;


    /// <summary>
    /// Used in tilemapGenerator. Specifies the y range of the composite map (Min - Max)
    /// </summary>
    public Vector2Int compMapYPos;
}