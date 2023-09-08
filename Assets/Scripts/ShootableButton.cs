using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnButtonHitEvent : UnityEvent { }

public class ShootableButton : MonoBehaviour
{
    public OnButtonHitEvent onButtonHit;

    private void Start()
    {
        onButtonHit.AddListener(DebugMessage);
    }

    private void DebugMessage()
    {
        Debug.Log("Hit the button");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.TryGetComponent(out Projectile projectile))
        {
            projectile.wallCollisionEvent.Invoke();
            onButtonHit.Invoke();
        }
    }
}
