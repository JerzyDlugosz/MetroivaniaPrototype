using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroRangeScript : MonoBehaviour
{
    [SerializeField]
    private AIDestinationSetter destinationSetter;
    [SerializeField]
    private bool isStationary;
    private BaseNPC baseNPC;

    private void Start()
    {
        baseNPC = GetComponentInParent<BaseNPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(isStationary)
            {
                baseNPC.canAttack = true;
            }
            else
            {
                baseNPC.canAttack = true;
                destinationSetter.target = collision.transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isStationary)
            {
                baseNPC.canAttack = false;
            }
            else
            {
                baseNPC.canAttack = false;
                //destinationSetter.target = null;
            }
        }
    }
}
