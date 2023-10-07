using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnTriggerEnterEvent : UnityEvent { }


public class MapBorderCollision : MonoBehaviour
{
    public Direction direction;

    public OnTriggerEnterEvent onTriggerEnterEvent;

    public bool isDisabled;

    public bool isADoor;

    public Vector2 additionalPlayerMove = Vector2.zero;

    public void disableTriggerEnter()
    {
        isDisabled = true;
    }

    public void enableTriggerEnter()
    {
        isDisabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isDisabled)
        {
            return;
        }
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player is exiting stage");
            onTriggerEnterEvent.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.parent.GetComponentInParent<CustomTilemap>().EnableAllTriggers();
    }
}

