using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionField : MonoBehaviour
{

    private List<BaseEntity> entintiesInExplosion = new List<BaseEntity>();
    private float explosionDamage;


    [SerializeField]
    private float explosionFieldStrength;

    public void OnInstantiate(float _explosionDamage)
    {
        StartCoroutine(explosionTimer());
        explosionDamage = _explosionDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out BaseEntity baseEntity))
        {
            //Debug.Log(collision.tag);
            entintiesInExplosion.Add(baseEntity);
        }
        else if(collision.transform.parent.TryGetComponent(out BaseEntity parentBaseEntity))
        {
            //Debug.Log(collision.tag);
            entintiesInExplosion.Add(parentBaseEntity);
        }
    }

    IEnumerator explosionTimer()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        EntityPush();
    }

    private void EntityPush()
    {
        foreach (var entity in entintiesInExplosion)
        {
            if(entity.TryGetComponent(out Player player))
            {
                Vector2 magnitude;
                float distance = Vector2.Distance(entity.transform.position, transform.position);
                if(distance < 1f)
                {
                    player.damageTakenEvent.Invoke(1);
                }

                float angle = MathExtensions.GetAngle(transform.position, entity.transform.position);
                magnitude = MathExtensions.GetAngleMagnitude(angle, false);
                magnitude = MathExtensions.GetReducedValue(magnitude, GetComponent<CircleCollider2D>().radius, distance);

                player.customRigidbody.AddRelativeForce(magnitude * explosionFieldStrength);
                continue;
            }

            if(entity.TryGetComponent(out BaseNPC baseNPC))
            {
                //Debug.Log(baseNPC.name);
                Vector2 magnitude;
                float distance = Vector2.Distance(entity.transform.position, transform.position);
                if (distance < 3f)
                {
                    baseNPC.TakeDamage(explosionDamage);
                }

                float angle = MathExtensions.GetAngle(transform.position, entity.transform.position);
                magnitude = MathExtensions.GetAngleMagnitude(angle, false);
                magnitude = MathExtensions.GetReducedValue(magnitude, GetComponent<CircleCollider2D>().radius, distance);

                baseNPC.ApplyForce(magnitude * explosionFieldStrength);
                continue;
            }
        }
        Destroy(gameObject);
    }

    
}
