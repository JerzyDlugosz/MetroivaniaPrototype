using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    public List<BossData> bossesSlayed = new List<BossData>();
    public List<Collectible> collectibles = new List<Collectible>();

    public void AddBoss(BossData _bossData)
    {
        BossData bossData = new BossData();
        bossData.bossId = _bossData.bossId;
        bossData.bossName = _bossData.bossName;
        bossData.bossXPos = _bossData.bossXPos;
        bossData.bossYPos = _bossData.bossYPos;

        bossesSlayed.Add(bossData);
    }

    public void AddCollectilbe(int id, int posX, int posY, Sprite sprite, CollectibleType collectibleType)
    {
        Collectible collectible = new Collectible();
        collectible.collectibleId = id;
        collectible.collectiblePosX = posX;
        collectible.collectiblePosY = posY;
        collectible.sprite = sprite;
        collectible.collectibleType = collectibleType;

        collectibles.Add(collectible);
    }

    public bool CheckBossID(BossData _bossData)
    {
        foreach (var bossData in bossesSlayed)
        {
            if(bossData.bossId == _bossData.bossId)
            {
                Debug.Log("TRUE?");
                return true;
            }
        }
        return false;
    }

    public bool CheckBossID(int BossId)
    {
        foreach (var bossData in bossesSlayed)
        {
            if (bossData.bossId == BossId)
            {
                return true;
            }
        }
        return false;
    }
}

