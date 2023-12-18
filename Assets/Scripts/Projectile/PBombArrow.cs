using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBombArrow : PlayerProjectile
{
    [SerializeField]
    private GameObject explosionField;
    [SerializeField]
    private float explosionDamage;
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

        entityCollisionEvent.AddListener(OnNPCCollision);

    }
    private void Update()
    {
        if (MainUpdate())
        {

        }
    }
    public void OnWallCollision()
    {
        GameObject eplosion = Instantiate(explosionField, transform.position, Quaternion.identity);
        eplosion.GetComponent<ExplosionField>().OnInstantiate(explosionDamage);
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(OnWallCollisionAudio);
        destroyEvent.Invoke();
    }

    public void OnNPCCollision(BaseNPC baseNPC)
    {
        GameObject eplosion = Instantiate(explosionField, transform.position, Quaternion.identity);
        eplosion.GetComponent<ExplosionField>().OnInstantiate(explosionDamage);
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(OnWallCollisionAudio);
        destroyEvent.Invoke();
    }
}
