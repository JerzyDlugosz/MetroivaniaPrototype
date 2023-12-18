using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArrowReloadSpeed : CollectibleGameObject
{
    void Start()
    {
        foreach (var item in GameManagerScript.instance.player.progressTracker.collectibles)
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
        GameManagerScript.instance.player.arrowReloadSpeedIncreaseEvent.Invoke();

        GameManagerScript.instance.player.progressTracker.AddCollectilbe(collectibleId, collectiblePositionX, collectiblePositionY, collectibleType);
        GameManagerScript.instance.player.progressTracker.OnArrowReloadSpeedAdd();

        ItemPickupPanel.instance.ShowPanel(collectibleName, collectibleDescription, collectibleSprite);

        Destroy(gameObject);
    }
}
