using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBossEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;
    [SerializeField]
    private bool basicSpriteRotation = true;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float attackSpeed = 3f;

    private float attackCooldown;
    [SerializeField]
    private bool isStunned;

    [SerializeField]
    public BossData bossData;

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

    [SerializeField]
    private GameObject stunIcon;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        attackCooldown = attackSpeed;
        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (isStunned)
        {
            return;
        }


        if (basicSpriteRotation)
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


        //if (attackCooldown <= 0)
        //{
        //    attackCooldown = attackSpeed;
        //    AttackLogic();
        //}

        if (attackCooldown <= 0)
        {
            GetRandomAttackPattern();
        }

    }
    private void OnHit(float damage)
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
       
    }

    public void OnDeath()
    {
        GameManagerScript.instance.player.progressTracker.AddBoss(bossData);
        GameManagerScript.instance.player.characterController.StopMovement(false);
        enemyParticleController.OnDeath();
        Destroy(stunIcon.gameObject);
        Destroy(gameObject);
    }

    public void OnStun(bool state)
    {
        StopAllCoroutines();
        stunIcon.SetActive(state);
        isStunned = state;
    }

    private void GetRandomAttackPattern()
    {
        int rand = Random.Range(0, 2);
        switch(rand)
        {
            case 0: AttackPattern1(false, 0);
                attackCooldown = 2f;
                break;
            case 1: AttackPattern2(true, 0.5f);
                attackCooldown = 5f;
                break;
            default:
                break;
        }

        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    private void AttackPattern1(bool timedAttack, float projectileDelay)
    {
        int projectileAmmount = 3;
        float projectileSpread = 15;

        if(timedAttack)
        {
            StartCoroutine(AttackDelay(projectileAmmount, projectileSpread, projectileDelay));
        }

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, -4f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));
            transform.rotation = targetRotation;

            GameObject projectile = Instantiate(projectilePrefab, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();
        }
    }


    private void AttackPattern2(bool timedAttack, float projectileDelay)
    {
        int projectileAmmount = 5;
        float projectileSpread = 0;

        if (timedAttack)
        {
            StartCoroutine(AttackDelay(projectileAmmount, projectileSpread, projectileDelay));
        }
    }



    private IEnumerator AttackDelay(int projectileAmmount, float projectileSpread, float projectileDelay)
    {

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, -4f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));
            transform.rotation = targetRotation;

            GameObject projectile = Instantiate(projectilePrefab, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

            yield return new WaitForSeconds(projectileDelay);
        }
    }


}
