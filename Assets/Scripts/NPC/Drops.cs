using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drops : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> npcDrops = new List<GameObject>();

    private GameObject GetRandomDrop()
    {
        int rand = Random.Range(0, npcDrops.Count);
        return npcDrops[rand];
    }

    public void DropCollectible(Vector3 position)
    {
        Instantiate(GetRandomDrop(), position, Quaternion.identity);
    }
}
