using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;



[Serializable]
public class OnRecallEvent : UnityEvent { }

[Serializable]
public class BeforeEndOfLifetimeEvent : UnityEvent { }


public class PlayerProjectile : Projectile
{
    [HideInInspector]
    public OnRecallEvent recallEvent;
    [HideInInspector]
    public BeforeEndOfLifetimeEvent beforeEndOfLifetimeEvent;

    [SerializeField]
    private float maxProjectileForceDistance = 5f;

    private Vector2 stoppedVelocity;
    private float stoppedAngularVelocity;

    [SerializeField]
    private float shakeStrenght;
    protected bool isStuckInWall = false;
    protected bool isShaking = false;

    public override void OnInstantiate(float angle, float distance, float multiplier)
    {
        float distanceMagnitude = MathF.Abs(distance);

        if (distanceMagnitude > maxProjectileForceDistance)
        {
            distanceMagnitude = maxProjectileForceDistance;
        }

        distanceMagnitude /= maxProjectileForceDistance;

        Debug.Log(distanceMagnitude);

        rb.AddRelativeForce((projectileForce * distanceMagnitude) * multiplier);

        Pushback(angle);

        baseDrag = rb.drag;

        recallEvent.AddListener(() => destroyEvent.Invoke());
        endOfLifetimeEvent.AddListener(() => destroyEvent.Invoke());
        entityCollisionEvent.AddListener(DestroyEventWithBaseNPC);
        destroyEvent.AddListener(() => transform.DOKill());

        stoppedEvent.AddListener(OnStop);
    }

    private void OnStop(bool state)
    {
        if (state)
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

    private void Pushback(float angle)
    {
        Vector2 pushbackMagnitude = MathExtensions.GetAngleMagnitude(angle, true);

        Debug.Log($"Pushback {pushbackMagnitude * projectilePushBack}");
        GameManagerScript.instance.player.customRigidbody.AddRelativeForce(pushbackMagnitude * projectilePushBack);
    }

    protected bool MainUpdate()
    {
        if (isStopped)
        {
            return false;
        }

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
        return true;
    }

    private void DestroyEventWithBaseNPC(BaseNPC baseNPC)
    {
        destroyEvent.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(collision.TryGetComponent(out BaseNPC baseNPC))
            {
                baseNPC.TakeDamage(projectileDamage);
                entityCollisionEvent.Invoke(baseNPC);
                return;
            }
            BaseNPC parentBaseNPC = collision.GetComponentInParent<BaseNPC>();
            parentBaseNPC.TakeDamage(projectileDamage);
            entityCollisionEvent.Invoke(parentBaseNPC);
        }
    }

    protected void ShakeArrow()
    {
        float angle = MathExtensions.WrapAngle(transform.eulerAngles.z);
        Debug.Log($"z angle: {angle}");
        Vector2 vector = MathExtensions.GetAngleMagnitude(angle, false);
        Debug.Log($"vector: {vector}");
        transform.DOShakePosition(1f, new Vector3(shakeStrenght * vector.y, shakeStrenght * vector.x, 0f)).SetEase(Ease.Linear);
    }
}
