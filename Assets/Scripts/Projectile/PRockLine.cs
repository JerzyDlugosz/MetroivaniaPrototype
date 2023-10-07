using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PRockLine : MonoBehaviour
{
    public List<GameObject> crackedRocks;
    public List<GameObject> allRocks;

    private void Update()
    {
        if(allRocks.Count <= 0)
        {
            Destroy(gameObject);
        }
    }
}
