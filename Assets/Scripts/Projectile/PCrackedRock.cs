using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnSingleRockHit : UnityEvent { }

public class PCrackedRock : HostileProjectile
{
    public OnSingleRockHit OnSingleRockHitEvent;

    public override void Start()
    {
        base.Start();

        wallCollisionEvent.AddListener(() => OnPlayerCollision());
        wallBounceEvent.AddListener(() => OnPlayerCollision());
    }

    public void OnPlayerCollision()
    {
        destroyEvent.Invoke();
    }

    protected override void AdditionalOnCollisionEnter(Collision2D collision)
    {
        if (collision.collider.CompareTag("Projectile"))
        {
            OnSingleRockHitEvent.Invoke();
            Debug.Log("Destroyed!");
        }
    }
}
