using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAiTargeter : MonoBehaviour
{
    [SerializeField]
    AIDestinationSetter destinationSetter;
    [HideInInspector]
    public Transform target;
    void Start()
    {
        if(destinationSetter == null)
        {
            target = GameManagerScript.instance.player.transform;
            return;
        }
        destinationSetter.target = GameManagerScript.instance.player.transform;
    }
}
