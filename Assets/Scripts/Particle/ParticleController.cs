using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    protected void StartParticle(GameObject particle)
    {
        if(particle != null)
        {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }

    protected void StartParticle(GameObject particle, Vector3 position)
    {
        if (particle != null)
        {
            Instantiate(particle, position, Quaternion.identity);
        }
    }
}
