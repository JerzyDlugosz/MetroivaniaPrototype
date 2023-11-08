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
    public bool onBossKilled;
    public int killedBossID;
    public bool onEventTriggered;
    private void Start()
    {
        if(onBossKilled)
        {
            if (GameManagerScript.instance.player.progressTracker.CheckBossID(killedBossID))
            {
                conditionMetEvent.Invoke();
            }
        }
    }

    public void OnEventTrigger()
    {
        conditionMetEvent.Invoke();
    }
}
