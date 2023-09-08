using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticleController : ParticleController
{
    public GameObject DeathParticle;

    public void OnDeath()
    {
        StartParticle(DeathParticle);
    }
}
