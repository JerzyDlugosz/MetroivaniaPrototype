using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;

    [SerializeField]
    private GameObject rangedProjectilePrefab;

    private float rangedAttackCooldown = 2f;

    [SerializeField]
    private float attackAngle;

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

        if (rangedAttackCooldown < 0)
        {
            AttackPattern1();
            GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
            rangedAttackCooldown = 2f;
        }
        
        if (rangedAttackCooldown > 0)
        {
            rangedAttackCooldown -= Time.deltaTime;
        }
    }

    public void OnStop(bool state)
    {

    }
    private void AttackPattern1()
    {
        int projectileAmmount = 5;
        float projectileSpread = 30;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, attackAngle + projectileRotation + (projectileSpread * i)));
            GameObject projectile = Instantiate(rangedProjectilePrefab, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

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
    }
}
