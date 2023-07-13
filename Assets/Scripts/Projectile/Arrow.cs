using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField]
    private Collider2D platformCollider;

    private void Start()
    {
        collisionEvent.AddListener(OnWallCollision);
        bounceEvent.AddListener(OnWallCollision);
    }

    public void OnWallCollision()
    {
        rb.bodyType = RigidbodyType2D.Static;
        projectileLifetime += 5f;
        platformCollider.enabled = true;
    }

    public void OnWallBounce()
    {

    }

}
