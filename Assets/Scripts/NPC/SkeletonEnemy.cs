using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;

    [SerializeField]
    private GameObject rangedProjectilePrefab;

    private float rangedAttackCooldown = 1f;

    [SerializeField]
    private GameObject bowSprite;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }
        UpdateSpriteDirection(isUsingRigidbody);
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

        if(canAttack)
        {
            if (rangedAttackCooldown < 1)
            {
                AimAtPlayer();
            }
            if (rangedAttackCooldown < 0)
            {
                AttackPattern1();
                GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
                rangedAttackCooldown = 2f;
            }
        }
        else
        {
            rangedAttackCooldown = 1f;
        }

        if (rangedAttackCooldown > 0)
        {
            rangedAttackCooldown -= Time.deltaTime;
        }
    }

    public void OnStop(bool state)
    {
       
    }

    private void AimAtPlayer()
    {
        Transform playerTransform = GameManagerScript.instance.player.transform;
        float angle = MathExtensions.GetAngle(transform.position, playerTransform.position);
        bowSprite.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void AttackPattern1()
    {
        int projectileAmmount = 1;
        float projectileSpread = 0;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = MathExtensions.GetAngle(transform.position, playerTransform.position);
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            bowSprite.transform.eulerAngles = new Vector3(0, 0, angle);
            GameObject projectile = Instantiate(rangedProjectilePrefab, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

        }

    }

    private void OnHit(float damage)
    {
        health -= damage;

        if (health <= 0f)
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
