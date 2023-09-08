using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAcidProjectile : HostileProjectile
{
    [SerializeField]
    private float bounceForceModifier;

    private void Start()
    {

        wallCollisionEvent.AddListener(() => Destroy(gameObject));
        wallBounceEvent.AddListener(() => OnWallBounce());
    }

    public void OnPlayerCollision()
    {
        destroyEvent.Invoke();
    }

    public void OnWallBounce()
    {
        Debug.Log(rb.velocity);
        rb.AddForce(new Vector2(0, 10 * bounceForceModifier)); 
    }
}
