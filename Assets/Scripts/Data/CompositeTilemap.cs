using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public MapList maplist;


#if (UNITY_EDITOR)

    public void SetConnectedTilemaps()
    {
        connectedTilemaps.Clear();
        foreach (Map map in maplist.maps)
        {
            if(map != null)
            {
                if (map.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
                {
                    Debug.Log($"comp tilemap: {comp.GetComponent<CustomTilemapData>().xPos}, {comp.GetComponent<CustomTilemapData>().yPos}");
                    if (comp.GetComponent<CustomTilemapData>().xPos >= compMapXPos.x && comp.GetComponent<CustomTilemapData>().xPos <= compMapXPos.y)
                    {

                        Debug.Log($"This for X ->{comp.GetComponent<CustomTilemapData>().xPos}: {compMapXPos.x}, {compMapXPos.y}");
                        if (comp.GetComponent<CustomTilemapData>().yPos >= compMapYPos.x && comp.GetComponent<CustomTilemapData>().yPos <= compMapYPos.y)
                        {
                            Debug.Log($"This for Y -> {comp.GetComponent<CustomTilemapData>().yPos}: {compMapYPos.x}, {compMapYPos.y}");
                            connectedTilemaps.Add(map);
                        }
                    }
                }
            }
        }
    }

#endif
}