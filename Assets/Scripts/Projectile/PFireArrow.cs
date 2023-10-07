using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFireArrow : PlayerProjectile
{
    [SerializeField]
    private float burningDamage = 1f;
    public override void Start()
    {
        base.Start();
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);

        endOfLifetimeEvent.AddListener(() => recallEvent.Invoke());
        entityCollisionEvent.AddListener(OnEnemyCollision);
    }
    private void Update()
    {
        if (MainUpdate())
        {

        }
    }

    public void OnWallCollision()
    {
        Debug.Log("HitWall");
        destroyEvent.Invoke();
    }

    public void OnEnemyCollision(BaseNPC baseNPC)
    {
        Debug.Log("HitEnemy");
        //apply burn debuff
        baseNPC.Burning(burningDamage, 5);
    }
}
