using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGlueArrow : PlayerProjectile
{
    [SerializeField]
    private Collider2D platformCollider;

    private void Start()
    {
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);

        endOfLifetimeEvent.AddListener(() => recallEvent.Invoke());
        entityCollisionEvent.AddListener(OnEnemyCollision);
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

    public void OnEnemyCollision(BaseNPC baseNPC)
    {
        //apply slow debuff
        baseNPC.Slowed();
    }
}
