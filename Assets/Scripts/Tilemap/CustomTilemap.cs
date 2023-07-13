using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomTilemap : MonoBehaviour
{
    [HideInInspector]
    public Map map;
    public GameObject outsideColliders;

    private float colliderSize;
    private float standartMapSize;

    public void SetupMap()
    {
        standartMapSize = GlobalData.mapSize;
        colliderSize = GlobalData.collisionSize;

        float[,] colliderPosition = {
            { transform.position.x + 0, transform.position.y + standartMapSize / 2 - (colliderSize - 1) },
            { transform.position.x + standartMapSize / 2 - (colliderSize - 1), transform.position.y + 0 },
            { transform.position.x + 0, transform.position.y - standartMapSize / 2 + (colliderSize - 1) },
            { transform.position.x - standartMapSize / 2 + (colliderSize - 1), transform.position.y + 0 }
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
            //StartCoroutine(disabledTime(child));

            child.GetComponent<BoxCollider2D>().enabled = true;
            child.GetComponent<MapBorderCollision>().disableTriggerEnter();
        }
    }

    public void EnableAllTriggers()
    {
        foreach (Transform child in outsideColliders.transform)
        {
            Debug.Log(child.name);
            child.GetComponent<MapBorderCollision>().isDisabled = false;
        }
    }

    IEnumerator disabledTime(Transform child)
    {
        yield return new WaitForSeconds(0.5f);
        child.GetComponent<BoxCollider2D>().enabled = true;
    }
}
