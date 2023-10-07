using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;

    [SerializeField]
    private bool isRanged = false;
    [SerializeField] 
    private GameObject rangedProjectilePrefab;

    private float rangedAttackCooldown = 2f;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if(isStopped)
        {
            return;
        }
        UpdateSpriteRotation(isUsingRigidbody);
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

        if(isRanged)
        {
            if(path.reachedEndOfPath)
            {
                if (rangedAttackCooldown < 0)
                {
                    AttackPattern1();
                    GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
                    rangedAttackCooldown = 2f;
                }
            }
            if(rangedAttackCooldown > 0)
            {
                rangedAttackCooldown -= Time.deltaTime;
            }
        }

    }

    public void OnStop(bool state)
    {
        path.canMove = !state;
    }
    private void AttackPattern1()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y, -4f);

        Transform playerTransform = GameManagerScript.instance.player.transform;

        float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        GameObject projectile = Instantiate(rangedProjectilePrefab, position, targetRotation);

        projectile.GetComponent<Projectile>().OnInstantiate();
        
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
    }
}
