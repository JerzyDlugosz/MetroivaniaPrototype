using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Map")]
public class Map : ScriptableObject
{
    [HideInInspector]
    public int mapID;
    [HideInInspector]
    public string mapName;
    [HideInInspector]
    public int xSize;
    [HideInInspector]
    public int ySize;
    [HideInInspector]
    public int mapXOffset;
    [HideInInspector]
    public int mapYOffset;
    [HideInInspector]
    public byte[,] mapTileArray;

    public AudioClip backgroundMusic;
    public GameObject mapPrefab;
    public List<Direction> mapDoors = new List<Direction>();
}
