using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;

    private float baseGravity;
    [SerializeField]
    private float outOfWaterGravity;
    public override void Start()
    {
        base.Start();

        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        baseGravity = NPCRigidbody.gravityScale;
    }

    private void Update()
    {
        UpdateSpriteRotation(false);

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
