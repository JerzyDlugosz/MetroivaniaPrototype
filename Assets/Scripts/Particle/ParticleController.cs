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
}
