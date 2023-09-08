using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    public List<BossData> bossesSlayed = new List<BossData>();

    public void AddBoss(BossData _bossData)
    {
        BossData bossData = new BossData();
        bossData.bossId = _bossData.bossId;
        bossData.bossName = _bossData.bossName;
        bossData.bossXPos = _bossData.bossXPos;
        bossData.bossYPos = _bossData.bossYPos;

        bossesSlayed.Add(bossData);
    }

    public bool CheckBossID(BossData _bossData)
    {
        foreach (var bossData in bossesSlayed)
        {
            if(bossData.bossId == _bossData.bossId)
            {
                return true;
            }
        }
        return false;
    }
}

