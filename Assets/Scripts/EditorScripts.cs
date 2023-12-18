using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#if (UNITY_EDITOR)
public class EditorScripts : MonoBehaviour
{
    public MapList maplist;

    private string folderPath = "Assets/GeneratedMapZone2";
    private string mapFolder = "Maps";
    private string prefabFolder = "Prefabs";

    [SerializeField]
    private TilemapGenerator tilemapGenerator;
    [SerializeField]
    private CollectibleList collectibleList;

    public void GetAllCollectiblesCount()
    {
        Debug.Log(FindObjectsOfType<CollectibleGameObject>().Length);
    }

    public void GetAllCollectiblesPositions()
    {
        CollectibleGameObject[] collectibleGameObjects = FindObjectsOfType<CollectibleGameObject>();

        List<int> xPositions = new List<int>();
        List<int> yPositions = new List<int>();

        for (int i = 0; i < collectibleGameObjects.Length; i++)
        {
            int xPos;
            int yPos;

            if (collectibleGameObjects[i].GetCollectibleType() == CollectibleType.StatUpgrade)
            {
                collectibleGameObjects[i].GetPosition(out xPos, out yPos);
                xPositions.Add(xPos);
                yPositions.Add(yPos);
            }
        }

        string xPosString = "";
        string yPosString = "";

        foreach (var item in xPositions)
        {
            xPosString += item.ToString() + ", ";
        }

        foreach (var item in yPositions)
        {
            yPosString += item.ToString() + ", ";
        }

        Debug.Log(xPosString);
        Debug.Log(yPosString);
    }

    public void SetStatCollectibleDataInGame()
    {
        CollectibleGameObject[] collectibleGameObjects = FindObjectsOfType<CollectibleGameObject>();
        //int permanentIndex = 0;
        //int arrowIndex = 10;
        int statIndex = 50;

        for (int i = 0; i < collectibleGameObjects.Length; i++)
        {


            //if (collectibleGameObjects[i].GetCollectibleType() == CollectibleType.ArrowType)
            //{
            //    collectibleGameObjects[i].collectibleId = arrowIndex;
            //    arrowIndex++;
            //}
            //if (collectibleGameObjects[i].GetCollectibleType() == CollectibleType.PermanentUpgrade)
            //{
            //    collectibleGameObjects[i].collectibleId = permanentIndex;
            //    permanentIndex++;
            //}
            if (collectibleGameObjects[i].GetCollectibleType() == CollectibleType.StatUpgrade)
            {
                CustomTilemapData tileData = collectibleGameObjects[i].GetComponentInParent<CustomTilemapData>();
                tileData.wasEdited = true;
                collectibleGameObjects[i].collectibleId = statIndex;
                collectibleGameObjects[i].SetPosition(tileData.xPos, tileData.yPos);

                statIndex++;
            }
        }
    }

    public void SetStatCollectibleDatanCollectibleList()
    {
        SetStatCollectibleDataInGame();
        CollectibleGameObject[] collectibleGameObjects = FindObjectsOfType<CollectibleGameObject>();

        //I shouldnt do this because I'll need to put this data in the other scene later
        collectibleList.collectibles.Clear();

        EditorUtility.SetDirty(collectibleList);

        for (int i = 0; i < collectibleGameObjects.Length; i++)
        {
            if (collectibleGameObjects[i].GetCollectibleType() == CollectibleType.StatUpgrade)
            {

                int posX = 0;
                int posY = 0;

                collectibleGameObjects[i].GetPosition(out posX, out posY);

                Collectible collectible = new()
                {
                    collectibleId = collectibleGameObjects[i].collectibleId,
                    collectiblePosX = posX,
                    collectiblePosY = posY
                };


                collectibleList.collectibles.Add(collectible);
            }
        }
    }

    public void GetAllPlayableTilemaps()
    {
        int i = 0;
        foreach (var item in maplist.maps)
        {
            if(item != null)
            {
                i++;
            }
        }
        Debug.Log(i);
    }

    public void SetCompositeTilemaps()
    {
        foreach (Map map in maplist.maps)
        {
            if (map != null)
            {
                if (map.mapPrefab.TryGetComponent<CompositeTilemap>(out CompositeTilemap comp))
                {
                    comp.maplist = maplist;
                    comp.SetConnectedTilemaps();
                }
            }
        }
    }

    public void SetDoors()
    {
        List<Map> maplistTemp = new List<Map>();

        string[] guids = AssetDatabase.FindAssets("t:Map", new string[] { $"{folderPath}/{mapFolder}" });

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            maplistTemp.Add(AssetDatabase.LoadAssetAtPath<Map>(path));
        }

        int guidIndex = 0;

        for (int j = 0; j < 20; j++)
        {
            for (int k = 0; k < 20; k++)
            {

            }
        }




        int i = 0;
        foreach (var item in maplist.maps)
        {
            if (item == null)
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
                    (i - 1) % 400,
                    (i + 1) % 400,
                    (i - 20) % 400,
                    (i + 20) % 400
                };

                foreach (var item2 in maplist.maps[i].mapDoors)
                {
                    Debug.Log($"1. [{i / 20}.{i % 20}] {item2}");
                }

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[0]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Down);

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[1]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Up);

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[2]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Left);

                if (comp.connectedTilemaps.Contains(maplist.maps[temp[3]]))
                    maplist.maps[i].mapDoors.Remove(Direction.Right);

                foreach (var item2 in maplist.maps[i].mapDoors)
                {
                    Debug.Log($"2. [{i / 20}.{i % 20}] {item2}");
                }
            }
            i++;
        } 
    }

    public void SetUnplayableMapVisibility(bool state)
    {
        foreach (var item in tilemapGenerator.currentTilemaps)
        {
            if(!item.GetComponent<CustomTilemapData>().isPlayable)
            {
                item.SetActive(state);
            }
        }
    }
}
#endif