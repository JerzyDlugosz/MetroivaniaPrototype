using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticleController : ParticleController
{
    public GameObject DeathParticle;
    public int particleAmmount = 1;
    public float particleSpread = 0;
    public bool isUsingCustomZPosition = false;
    public int customZPosition = 0;

    public void OnDeath()
    {
        for (int i = 0; i < particleAmmount; i++)
        {
            float zposition = transform.position.z;
            if (isUsingCustomZPosition)
                zposition = customZPosition;
            Vector3 particleSpawnPosition = new Vector3(transform.position.x + Random.Range(-particleSpread,particleSpread), transform.position.y + Random.Range(-particleSpread, particleSpread), zposition);
            StartParticle(DeathParticle, particleSpawnPosition);
        }
    }
}
