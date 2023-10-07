using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PShockwave : HostileProjectile
{
    public override void Start()
    {
        base.Start();

        wallCollisionEvent.AddListener(() => OnPlayerCollision());
        wallBounceEvent.AddListener(() => OnPlayerCollision());
        stoppedEvent.AddListener(OnStop);
    }

    public void OnPlayerCollision()
    {
        destroyEvent.Invoke();
    }

    private void OnStop(bool state)
    {
        GetComponent<CustomSpriteAnimation>().stopAnimation = state;
    }
}
