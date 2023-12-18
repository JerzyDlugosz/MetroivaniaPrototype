using DG.Tweening;
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
    private float attackSpeed = 1f;

    private float attackCooldown;
    [SerializeField]
    private bool isStunned;

    [SerializeField]
    public BossData bossData;

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

    [SerializeField]
    private GameObject stunIcon;
    [SerializeField]
    private Transform centerOfRoom;
    public SpriteRenderer neckSpriteRenderer;
    public List<Sprite> neckSprites;
    public List<Transform> headPositions = new List<Transform>();
    public List<Sprite> onStunSprites;
    private int previousHeadIndex = -1;

    Transform playerTransform;

    [SerializeField]
    private List<SpriteRenderer> bossSpriteRenderers;

    [SerializeField]
    public List<GameObject> projectiles;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        attackCooldown = 3f;
        stoppedEvent.AddListener(OnStop);

        playerTransform = GameManagerScript.instance.player.transform;
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }

        if (isStunned)
        {
            return;
        }

        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;


        if (basicSpriteRotation)
            UpdateSpriteDirection(isUsingRigidbody);

        UpdateBossLocationAndRotation();


        if (attackCooldown <= 0)
        {
            GetRandomAttackPattern();
        }

    }
    private void OnHit(float damage)
    {
        health -= damage;
        float scale = (maxHealth - health) / maxHealth;
        attackSpeed =  1 + scale / 2;
        for (int i = 0; i < bossSpriteRenderers.Count - 1; i++)
        {
            bossSpriteRenderers[i].material.SetFloat(DamageScaleID, 1 + scale);
        }
        if (health <= 0f)
        {
            onNPCDeath.Invoke();
        }

        StartCoroutine(DamageTimer());
    }

    public void OnStop(bool state)
    {
       
    }

    public override void Invincibility(bool state)
    {
        for (int i = 0; i < bossSpriteRenderers.Count - 1; i++)
        {
            if (state)
            {
                bossSpriteRenderers[i].material.SetFloat(InvincibilityID, 1);
            }
            else
            {
                bossSpriteRenderers[i].material.SetFloat(InvincibilityID, 0);
            }
        }
        isInvincible = state;
    }

    private void UpdateBossLocationAndRotation()
    {
        float bossToPlayerDistance = GameManagerScript.instance.player.transform.position.x - centerOfRoom.position.x;

        int index = (int)((Mathf.Max(-10, Mathf.Min(9, bossToPlayerDistance)) / 5) + 2);

        if(previousHeadIndex != index)
        {
            transform.position = headPositions[index].position;
            neckSpriteRenderer.sprite = neckSprites[index];
        }

        float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void OnDeath()
    {
        GameManagerScript.instance.player.progressTracker.AddBoss(bossData);
        GameManagerScript.instance.player.characterController.StopMovement(false);
        enemyParticleController.OnDeath();
        Destroy(stunIcon);
        foreach (var item in bossSpriteRenderers)
        {
            Destroy(item);
        }
        Destroy(gameObject);
    }

    public void OnStun(bool state)
    {
        StopAllCoroutines();
        RemoveProjectiles();
        stunIcon.SetActive(state);
        isStunned = state;
        if (state)
            attackCooldown = 2f;
    }

    public void FadeIn()
    {
        foreach (var item in bossSpriteRenderers)
        {
            item.DOFade(1, 2f);
        }
        GameManagerScript.instance.cameraHolder.DOShakePosition(2f, 1.5f);
    }

    public void SetAlpha(float alpha)
    {
        foreach (var item in bossSpriteRenderers)
        {
            item.color = new Color(item.color.r, item.color.g, item.color.b, alpha);
        }
    }


    private void GetRandomAttackPattern()
    {
        int rand = Random.Range(0, 2);
        switch(rand)
        {
            case 0: AttackPattern1(false, 0);
                attackCooldown = 2f / attackSpeed;
                break;
            case 1: AttackPattern2(true, 0.5f);
                attackCooldown = 4f / attackSpeed;
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

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(projectilePrefab, position, targetRotation);
            projectiles.Add(projectile);
            projectile.GetComponent<Projectile>().destroyEvent.AddListener(() =>
            {
                RemoveProjectileFromProjectiles(projectile);
            });
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

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(projectilePrefab, position, targetRotation);
            projectiles.Add(projectile);
            projectile.GetComponent<Projectile>().OnInstantiate();
            projectile.GetComponent<Projectile>().destroyEvent.AddListener(() => 
            { 
                RemoveProjectileFromProjectiles(projectile);
            });
            yield return new WaitForSeconds(projectileDelay);
        }
    }

    private void RemoveProjectileFromProjectiles(GameObject item)
    {
        projectiles.Remove(item);

    }

    private void RemoveProjectiles()
    {
        for (int i = 0; i < projectiles.Count; i++)
        {
            Destroy(projectiles[i]);
        }

        projectiles.Clear();
    }


}
