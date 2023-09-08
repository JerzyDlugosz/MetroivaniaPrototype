using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEarthSpirit : CollectibleGameObject
{ 
    void Start()
    {
        foreach (var item in GameManagerScript.instance.player.collectedItems.collectibles)
        {
            if (item.collectibleId == collectibleId)
            {
                Destroy(gameObject);
                return;
            }
        }
        collectEvent.AddListener(OnCollect);
    }

    void OnCollect()
    {
        GameManagerScript.instance.player.playerData.EarthSpirit = true;

        GameManagerScript.instance.player.collectedItems.AddCollectilbe(collectibleId, collectiblePositionX, collectiblePositionY);

        Destroy(gameObject);
    }
}
