using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBombArrow : PlayerProjectile
{
    [SerializeField]
    private GameObject explosionField;
    public override void Start()
    {
        base.Start();
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());
        endOfLifetimeEvent.AddListener(() => projectileParticleController.OnCollision());
        wallBounceEvent.AddListener(() => projectileParticleController.OnCollision());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);

        endOfLifetimeEvent.AddListener(OnWallCollision);
    }
    private void Update()
    {
        if (MainUpdate())
        {

        }
    }
    public void OnWallCollision()
    {
        //fancy explosion thing
        GameObject eplosion = Instantiate(explosionField, transform.position, Quaternion.identity);
        eplosion.GetComponent<ExplosionField>().OnInstantiate();
        destroyEvent.Invoke();
    }
}
