using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;



[Serializable]
public class OnRecallEvent : UnityEvent { }


public class PlayerProjectile : Projectile
{
    public OnRecallEvent recallEvent;

    public override void OnInstantiate(float angle)
    {
        //rb.AddTorque(angle, ForceMode2D.Impulse);
        rb.AddRelativeForce(projectileForce);

        Pushback(angle);

        baseDrag = rb.drag;

        destroyEvent.AddListener(() => Destroy(gameObject));

        recallEvent.AddListener(() => destroyEvent.Invoke());
        entityCollisionEvent.AddListener(DestroyEventWithBaseNPC);
    }

    private void Pushback(float angle)
    {
        Vector2 pushbackMagnitude = MathExtensions.GetAngleMagnitude(angle, true);

        Debug.Log($"Pushback {pushbackMagnitude * projectilePushBack}");
        GameManagerScript.instance.player.customRigidbody.AddRelativeForce(pushbackMagnitude * projectilePushBack);
    }

    protected void MainUpdate()
    {
        WaterCheck();

        if (rb.velocity.magnitude > 0)
        {
            transform.right = rb.velocity.normalized;
        }


        projectileLifetime -= Time.deltaTime;

        if (projectileLifetime < 0)
        {
            endOfLifetimeEvent.Invoke();
        }

        debugRays();
    }

    private void DestroyEventWithBaseNPC(BaseNPC baseNPC)
    {
        destroyEvent.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            BaseNPC baseNPC = collision.GetComponent<BaseNPC>();
            baseNPC.TakeDamage(projectileDamage);
            entityCollisionEvent.Invoke(baseNPC);
        }
    }
}
