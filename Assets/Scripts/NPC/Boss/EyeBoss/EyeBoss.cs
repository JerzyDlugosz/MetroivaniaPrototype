using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EyeBoss : BaseNPC
{
    [SerializeField]
    protected float attackSpeed = 4f;
    protected float attackCooldown;
    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

    [SerializeField]
    protected EyeBossComposite eyeComposite;

    #region MovementFields
    [SerializeField]
    protected Transform movementZone;
    [SerializeField]
    protected Vector3 nextMovementPoint;
    [SerializeField]
    protected Vector3 force;
    [SerializeField]
    protected float maxSpeed;
    [SerializeField, Range(0, 1)]
    private float velocityDecrease;
    [SerializeField]
    private float reachedPointDistance = 2;
    #endregion

    protected void OnHit(float damage)
    {
        health -= damage;
        float scale = (maxHealth - health) / (maxHealth / 4);
        spriteRenderer.material.SetFloat(DamageScaleID, 1 + scale);
        if (health < 0f)
        {
            onNPCDeath.Invoke();
        }

        StartCoroutine(DamageTimer());
    }

    public void OnStop(bool state)
    {
        if (state)
        {
            transform.DOPause();
        }

        if (!state)
        {
            transform.DOPlay();
        }
    }

    public void OnDeath()
    {
        enemyParticleController.OnDeath();
        Destroy(gameObject);
    }

    protected void MovementLogic()
    {
        force = (new Vector3(nextMovementPoint.x, nextMovementPoint.y, transform.localPosition.z) - transform.localPosition).normalized * speed;
        NPCRigidbody.AddForce(force);
        if (Mathf.Abs(NPCRigidbody.velocity.x) >= maxSpeed)
        {
            NPCRigidbody.velocity = new Vector2(NPCRigidbody.velocity.x * velocityDecrease, NPCRigidbody.velocity.y);
        }
        if (Mathf.Abs(NPCRigidbody.velocity.y) >= maxSpeed)
        {
            NPCRigidbody.velocity = new Vector2(NPCRigidbody.velocity.x, NPCRigidbody.velocity.y * velocityDecrease);
        }

        if(Vector2.Distance(transform.localPosition, nextMovementPoint) < reachedPointDistance)
        {
             ChooseNextMovementPoint();
        }
    }

    protected void ChooseNextMovementPoint()
    {
        nextMovementPoint = GetRandomMoveLocation();
    }

    protected Vector3 GetRandomMoveLocation()
    {
        float xBoundary = movementZone.localScale.x / 2;
        float yBoundary = movementZone.localScale.y / 2;

        return new Vector3(Random.Range(movementZone.localPosition.x - xBoundary, movementZone.localPosition.x + xBoundary),
                  Random.Range(movementZone.localPosition.y - yBoundary, movementZone.localPosition.y + yBoundary), transform.localPosition.z);

    }

    protected Vector3 GetRandomYMoveLocation()
    {
        float xBoundary = movementZone.localScale.x / 2;
        float yBoundary = movementZone.localScale.y / 2;

        return new Vector3(movementZone.localPosition.x + xBoundary,
                  Random.Range(movementZone.localPosition.y - yBoundary, movementZone.localPosition.y + yBoundary), transform.localPosition.z);

    }
}
