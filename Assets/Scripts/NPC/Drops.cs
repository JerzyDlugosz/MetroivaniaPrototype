using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int rand = Random.Range(0, npcDrops.Count);
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
            Instantiate(GetRandomDrop(), dropLocation.position, Quaternion.identity);
        }
        else
        {
            float temp = ((npcDrops.Count - 1) / 2f) - (npcDrops.Count - 1);
            Vector3 vec = new Vector3(dropLocation.position.x + temp, dropLocation.position.y, dropLocation.position.z);
            for (int i = 0; i < npcDrops.Count; i++)
            {
                Instantiate(GetDrop(i), new Vector3(vec.x + i, vec.y, vec.z), Quaternion.identity);
            }
        }
    }
}
