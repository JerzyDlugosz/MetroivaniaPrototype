using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWave : HostileProjectile
{
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
}
