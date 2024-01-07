using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class HostileProjectile : Projectile
{
    private Vector2 stoppedVelocity;
    private float stoppedAngularVelocity;


    public override void OnInstantiate()
    {
        rb.AddRelativeForce(projectileForce);
        baseDrag = rb.drag;

        entityCollisionEvent.AddListener(DestroyEventWithBaseNPC);
        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if(isStopped)
        {
            return;
        }

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

    private void OnStop(bool state)
    {
        if(state) 
        {
            stoppedVelocity = rb.velocity;
            stoppedAngularVelocity = rb.angularVelocity;
            rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = stoppedVelocity;
            rb.angularVelocity = stoppedAngularVelocity;
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
            //Debug.Log("YOU GOT HIT!");
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(projectileDamage);
            entityCollisionEvent.Invoke(collision.GetComponentInParent<BaseNPC>());
        }
        AdditionalOnTriggerEnter(collision);
    }

    protected virtual void AdditionalOnTriggerEnter(Collider2D collision)
    {

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
