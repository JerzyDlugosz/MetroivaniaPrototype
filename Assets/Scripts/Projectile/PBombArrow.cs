using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBombArrow : PlayerProjectile
{
    [SerializeField]
    private GameObject explosionField;
    private void Start()
    {
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());
        endOfLifetimeEvent.AddListener(() => projectileParticleController.OnCollision());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);

        endOfLifetimeEvent.AddListener(OnWallCollision);
    }
    private void Update()
    {
        MainUpdate();
    }
    public void OnWallCollision()
    {
        //fancy explosion thing
        GameObject eplosion = Instantiate(explosionField, transform.position, Quaternion.identity);
        eplosion.GetComponent<ExplosionField>().OnInstantiate();
        destroyEvent.Invoke();
    }
}
