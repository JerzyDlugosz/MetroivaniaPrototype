using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticleController : ParticleController
{
    public GameObject recallParticle;
    public GameObject collisionParticle;
    public GameObject breakParticle;

    public void OnCollision()
    {
        StartParticle(collisionParticle);
    }

    public void OnRecall()
    {
        StartParticle(recallParticle);
    }

    public void OnBreak()
    {
        StartParticle(breakParticle);
    }
}
