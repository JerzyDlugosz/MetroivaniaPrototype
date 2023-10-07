using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHealth : CollectibleGameObject
{
    public float healthAdded;

    void Start()
    {
        collectEvent.AddListener(OnCollect);
    }

    void OnCollect()
    {
        GameManagerScript.instance.player.healthPickupEvent.Invoke(healthAdded);

        Destroy(gameObject);
    }
}
