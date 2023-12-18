using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDefaultEnemy : HostileProjectile
{
    public override void Start()
    {
        base.Start();

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);
    }

    public void OnPlayerCollision()
    {
        destroyEvent.Invoke();
    }

    public void OnWallCollision()
    {
        destroyEvent.Invoke();
    }
}
