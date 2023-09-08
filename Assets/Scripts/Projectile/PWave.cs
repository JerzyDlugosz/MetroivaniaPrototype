using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWave : HostileProjectile
{
    private void Start()
    {

        wallCollisionEvent.AddListener(() => OnPlayerCollision());
        wallBounceEvent.AddListener(() => OnPlayerCollision());
    }

    public void OnPlayerCollision()
    {
        destroyEvent.Invoke();
    }
}
