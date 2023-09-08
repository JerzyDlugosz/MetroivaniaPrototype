using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWater : HostileProjectile
{    
    private void Start()
    {

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
