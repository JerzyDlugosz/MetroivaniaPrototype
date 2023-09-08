using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class HostileProjectile : Projectile
{
    public override void OnInstantiate()
    {
        rb.AddRelativeForce(projectileForce);
        baseDrag = rb.drag;

        destroyEvent.AddListener(() => Destroy(gameObject));
        entityCollisionEvent.AddListener(DestroyEventWithBaseNPC);
    }

    private void Update()
    {
        WaterCheck();

        if (rb.velocity.magnitude > 0)
        {
            transform.right = rb.velocity.normalized;
        }

        projectileLifetime -= Time.deltaTime;

        if (projectileLifetime < 0)
        {
            destroyEvent.Invoke();
        }
    }

    private void DestroyEventWithBaseNPC(BaseNPC baseNPC)
    {
        destroyEvent.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("YOU GOT HIT!");
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(projectileDamage);
            entityCollisionEvent.Invoke(collision.GetComponentInParent<BaseNPC>());
        }
    }

    public void StopCollision(float time)
    {
        StartCoroutine(CollisionTimer(time));
    }

    IEnumerator CollisionTimer(float time)
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(time);
        GetComponent<Collider2D>().enabled = true;
    }
}
