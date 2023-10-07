using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGlueArrow : PlayerProjectile
{
    [SerializeField]
    private Collider2D platformCollider;

    public override void Start()
    {
        base.Start();
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());
        endOfLifetimeEvent.AddListener(() => projectileParticleController.OnBreak());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);
        beforeEndOfLifetimeEvent.AddListener(() => ShakeArrow());

        entityCollisionEvent.AddListener(OnEnemyCollision);
    }
    private void Update()
    {
        if (MainUpdate())
        {
            if (isStuckInWall)
            {
                if (projectileLifetime < 1 && !isShaking)
                {
                    isShaking = true;
                    beforeEndOfLifetimeEvent.Invoke();
                }
            }
        }
    }

    public void OnWallCollision()
    {
        rb.bodyType = RigidbodyType2D.Static;
        projectileLifetime += 5f;
        platformCollider.transform.localRotation = Quaternion.Euler(0f, 0f, -gameObject.transform.localEulerAngles.z);
        platformCollider.enabled = true;
        isStuckInWall = true;
    }

    public void OnEnemyCollision(BaseNPC baseNPC)
    {
        //apply slow debuff
        baseNPC.Slowed();
    }
}
