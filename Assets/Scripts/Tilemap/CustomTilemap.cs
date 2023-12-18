using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class CustomTilemap : MonoBehaviour
{
    [HideInInspector]
    public Map map;
    public GameObject outsideColliders;

    private float colliderSize;
    private float standartMapSize;

    public Zone zone;
    public SpecialRoom specialRoom;

    public void FirstSetupMap()
    {
        standartMapSize = GlobalData.maxTimemaps;
        colliderSize = GlobalData.MapDoorCollisionSize;

        float[,] colliderPosition = {
            { transform.position.x + 0, transform.position.y + (standartMapSize / 2) - (colliderSize - 1) },
            { transform.position.x + (standartMapSize / 2) - (colliderSize - 1), transform.position.y + 0 },
            { transform.position.x + 0, transform.position.y - (standartMapSize / 2) + (colliderSize - 1) },
            { transform.position.x - (standartMapSize / 2) + (colliderSize - 1), transform.position.y + 0 }
        };

        float[,] colliderScale = {
            { standartMapSize, colliderSize },
            { colliderSize, standartMapSize },
            { standartMapSize, colliderSize },
            { colliderSize, standartMapSize }
        };

        //for (int i = 0; i < outsideColliders.transform.childCount; i++)
        //{
        //    Transform child = outsideColliders.transform.GetChild(i).transform;
        //    child.position = new Vector2(colliderPosition[i, 0], colliderPosition[i, 1]);
        //    child.localScale = new Vector2(colliderScale[i, 0], colliderScale[i, 1]);
        //}

        foreach (Transform child in outsideColliders.transform)
        {
            MapBorderCollision childCollision = child.GetComponent<MapBorderCollision>();
            child.position = new Vector2(colliderPosition[(int)childCollision.direction, 0], colliderPosition[(int)childCollision.direction, 1]);
            child.localScale = new Vector2(colliderScale[(int)childCollision.direction, 0], colliderScale[(int)childCollision.direction, 1]);
        }

        foreach (Transform child in outsideColliders.transform)
        {
            //child.GetComponent<MapBorderCollision>().disableTriggerEnter();
            child.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public bool OnRoomEnter()
    {
        if (TryGetComponent(out BossRoom comp))
        {
            comp.OnBossRoomEnter();
            return true;
        }
        else
        {
            CameraData cameraData = Camera.main.GetComponent<CameraData>();
            cameraData.CameraXBoundaryAdditionalOffset = cameraData.baseCameraXBoundaryAdditionalOffset;
            cameraData.CameraYBoundaryAdditionalOffset = cameraData.baseCameraYBoundaryAdditionalOffset;

            GameManagerScript.instance.player.mainCamera.GetComponent<UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera>().assetsPPU = cameraData.baseAssetsPPU;
            return false;
        }
    }

    public void EnableAllTriggers()
    {
        foreach (Transform child in outsideColliders.transform)
        {
            //Debug.Log(child.name);
            child.GetComponent<MapBorderCollision>().enableTriggerEnter();
        }
    }

    public void DisableAllTriggers(float time)
    {
        foreach (Transform child in outsideColliders.transform)
        {
            child.GetComponent<MapBorderCollision>().disableTriggerEnter();
            //disabledTime(child, time);
        }
    }

    IEnumerator disabledTime(Transform child, float time)
    {
        yield return new WaitForSeconds(time);
        child.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void UpdatePosValues()
    {
        string toBeSearched = "[";
        int ix = name.IndexOf(toBeSearched);

        string toBeSearched2 = ",";
        int ix2 = name.IndexOf(toBeSearched2);

        string toBeSearched3 = "]";
        int ix3 = name.IndexOf(toBeSearched3);


        string xPos = name.Substring(ix + 1, ix2 - 1);
        Debug.Log(xPos);
        string yPos = name.Substring(ix2 + 1, ix3 - (ix2 + 1));
        Debug.Log(yPos);

        
        GetComponent<CustomTilemapData>().xPos = int.Parse(xPos);
        GetComponent<CustomTilemapData>().yPos = int.Parse(yPos);
    }
}
