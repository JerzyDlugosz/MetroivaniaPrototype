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
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(OnWallCollisionAudio);
        destroyEvent.Invoke();
    }

    public void OnEnemyCollision(BaseNPC baseNPC)
    {
        baseNPC.Burning(burningDamage, 5);
    }
}
