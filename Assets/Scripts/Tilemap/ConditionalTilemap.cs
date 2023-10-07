using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnConditionMetEvent : UnityEvent { }
public class ConditionalTilemap : MonoBehaviour
{
    public OnConditionMetEvent conditionMetEvent;
    public int killedBossID;
    public bool willBeDestroyed;
    private void Start()
    {
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(killedBossID))
        {
            conditionMetEvent.Invoke();
        }
    }
}
