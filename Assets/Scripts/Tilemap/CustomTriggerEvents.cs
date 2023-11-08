using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnTriggerEntry : UnityEvent { }
public class CustomTriggerEvents : MonoBehaviour
{
    [SerializeField]
    private bool destroyOnEntry = true;
    public OnTriggerEntry onTriggerEntry;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            onTriggerEntry.Invoke();
            if(destroyOnEntry)
            {
                Destroy(gameObject);
            }
        }
    }
}
