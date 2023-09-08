using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBowAndArrow : CollectibleGameObject
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
        GameManagerScript.instance.player.OnBowUnlocked(true);

        GameManagerScript.instance.player.arrowTypeCollectedEvent.Invoke();

        GameManagerScript.instance.player.arrowCapacityIncreaseEvent.Invoke();

        GameManagerScript.instance.player.collectedItems.AddCollectilbe(collectibleId, collectiblePositionX, collectiblePositionY);

        Destroy(gameObject);
    }
}