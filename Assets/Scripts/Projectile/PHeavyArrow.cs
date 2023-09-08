using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PHeavyArrow : PlayerProjectile
{
    [SerializeField]
    private Collider2D platformCollider;

    private void Start()
    {
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallBounce);

        endOfLifetimeEvent.AddListener(() => recallEvent.Invoke());
    }
    private void Update()
    {
        MainUpdate();
    }

    public void OnWallCollision()
    {
        rb.bodyType = RigidbodyType2D.Static;
        projectileLifetime += 5f;
        platformCollider.transform.localRotation = Quaternion.Euler(0f, 0f, -gameObject.transform.localEulerAngles.z);
        platformCollider.enabled = true;
    }

    public void OnWallBounce()
    {

    }
}
