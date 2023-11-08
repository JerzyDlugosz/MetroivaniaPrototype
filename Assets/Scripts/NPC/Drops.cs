using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Drops : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> npcDrops = new List<GameObject>();
    public bool randomizeDrops = true;
    public Transform dropLocation;

    private void Start()
    {
        if(dropLocation == null)
        {
            dropLocation = transform;
        }
    }
    private GameObject GetRandomDrop()
    {
        int rand = UnityEngine.Random.Range(0, npcDrops.Count);
        return npcDrops[rand];
    }

    private GameObject GetDrop(int dropNumber)
    {
        return npcDrops[dropNumber];
    }

    public void DropCollectible()
    {
        if (randomizeDrops)
        {
            GameObject drop = Instantiate(GetRandomDrop(), dropLocation.position, Quaternion.identity);

            //if (GetComponentInParent<BossRoom>())
            //{
            //    drop.GetComponent<CollectibleGameObject>().collectEvent.AddListener(() =>
            //    {
            //        GetComponentInParent<BossRoom>().OnDropPickupEvent.Invoke();
            //    });
            //}
        }
        else
        {
            float temp = ((npcDrops.Count - 1) / 2f) - (npcDrops.Count - 1);
            Vector3 vec = new Vector3(dropLocation.position.x + temp, dropLocation.position.y, dropLocation.position.z);
            for (int i = 0; i < npcDrops.Count; i++)
            {
                GameObject drop = Instantiate(GetDrop(i), new Vector3(vec.x + i, vec.y, vec.z), Quaternion.identity);

                //if (GetComponentInParent<BossRoom>())
                //{
                //    Debug.LogError("YES!");
                //    drop.GetComponent<CollectibleGameObject>().collectEvent.AddListener(() =>
                //    {
                //        Debug.LogError("befor!");
                //        GetComponentInParent<BossRoom>().OnDropPickupEvent.Invoke();
                //    });
                //}
            }
        }
    }
}
