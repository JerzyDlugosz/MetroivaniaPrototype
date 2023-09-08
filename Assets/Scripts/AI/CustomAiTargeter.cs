using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAiTargeter : MonoBehaviour
{
    [SerializeField]
    AIDestinationSetter destinationSetter;
    [SerializeField]
    private destinationTarget destinationTarget;

    public Transform target;
    void Start()
    {
        if(destinationTarget == destinationTarget.Player)
        {
            destinationSetter.target = GameManagerScript.instance.player.transform;
        }
        else
        {
            if (target == null)
            {
                return;
            }
            if (destinationSetter == null)
            {
                return;
            }
        }
    }
}

public enum destinationTarget
{
    None,
    Player,
}
