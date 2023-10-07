using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private float baseGravity;
    [SerializeField]
    private float outOfWaterGravity;

    public override void Start()
    {
        base.Start();

        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        stoppedEvent.AddListener(OnStop);
    }

    public void OnStop(bool state)
    {
        path.canMove = !state;
    }


    private void Update()
    {
        if (isStopped)
        {
            return;
        }
        UpdateSpriteRotation(false);

        if (!inWater)
        {
            NPCRigidbody.gravityScale = outOfWaterGravity;
            path.canMove = false;
        }
        else
        {
            path.canMove = true;
            NPCRigidbody.gravityScale = baseGravity;
        }


        if (!isUsingVelocityForAnimation)
        {
            spriteAnimation.UpdateAnimationFrame();
        }
        else
        {
            if (NPCRigidbody != null)
            {
                spriteAnimation.UpdateAnimationFrame(NPCRigidbody.velocity.x);
                return;
            }
            spriteAnimation.UpdateAnimationFrame(path.velocity.x);
        }
    }
    private void OnHit(float damage)
    {
        health -= damage;

        if (health < 0f)
        {
            onNPCDeath.Invoke();
        }

        StartCoroutine(DamageTimer());
    }

    public void OnDeath()
    {
        enemyParticleController.OnDeath();
        Destroy(gameObject);
    }
}
