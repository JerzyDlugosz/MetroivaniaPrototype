using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BirdBoss : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;
    [SerializeField]
    private bool basicSpriteRotation = true;

    [SerializeField]
    private float attackSpeed = 4f;
    private float attackCooldown;

    [SerializeField]
    public BossData bossData;

    private bool stopCoroutines = false;
    private bool stopAttacks = false;

    #region AttackPattern1Fields
    [SerializeField]
    private GameObject attackPattern1ProjectilePrefab;
    #endregion

    #region AttackPattern2Fields
    [SerializeField]
    private GameObject attackPattern2Laser;
    [SerializeField]
    private SpriteRenderer attackPattern2LaserSpriteRenderer;
    [SerializeField]
    private BoxCollider2D attackPattern2LaserCollider;
    [SerializeField]
    private List<Sprite> attackPattern2LaserSprites = new List<Sprite>();
    #endregion

    #region AttackPattern3Fields
    [SerializeField]
    private GameObject attackPattern3BatEnemyPrefab;
    [SerializeField]
    private GameObject attackPattern3RamEnemyPrefab;
    [SerializeField]
    private Vector2 attackPattern3SpawnOffset;
    [SerializeField, Range(0,1)]
    private List<float> healthTreshholdsPercent;
    private int healthTreshholdIndex = 0;
    [SerializeField]
    private int attackPattern3EnemiesCount;
    #endregion

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        attackCooldown = attackSpeed;
        stoppedEvent.AddListener(OnStop);

        attackPattern2Laser.transform.DOLocalRotate(new Vector3(0, 0, -360f), 10f, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear).SetLoops(-1);
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }

        if(healthTreshholdIndex < healthTreshholdsPercent.Count)
        {
            if (health < maxHealth * healthTreshholdsPercent[healthTreshholdIndex])
            {
                healthTreshholdIndex++;
                StartCoroutine(AttackPattern3());
            }
        }

        UpdateSpriteRotation(false);
        spriteAnimation.UpdateAnimationFrame();


        if (stopAttacks)
            return;

        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0)
        {
            attackCooldown = attackSpeed;
            AttackLogic();
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
        stopCoroutines = state;

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

    private void AttackLogic()
    {
        int rand = Random.Range(0, 2);

        switch (rand)
        {
            case 0:
                attackCooldown += 6f;
                StartCoroutine(AttackPattern1());
                break;
            case 1:
                attackCooldown += 8f;
                StartCoroutine(AttackPattern2());
                break;
        }
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    IEnumerator AttackPattern1()
    {
        int projectileAmmount = 5;
        float projectileSpread = 15;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, -4f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(attackPattern1ProjectilePrefab, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator AttackPattern2()
    {
        SetLaser(0.25f, 1);
        //attackPattern2LaserSpriteRenderer.color = new Color(attackPattern2LaserSpriteRenderer.color.r, attackPattern2LaserSpriteRenderer.color.g, attackPattern2LaserSpriteRenderer.color.b, 0.5f);
        yield return new WaitForSeconds(0.5f);
        SetLaser(0.5f, 2);
        yield return new WaitForSeconds(0.5f);
        SetLaser(0.75f, 3);
        //attackPattern2LaserSpriteRenderer.color = new Color(attackPattern2LaserSpriteRenderer.color.r, attackPattern2LaserSpriteRenderer.color.g, attackPattern2LaserSpriteRenderer.color.b, 1f);
        yield return new WaitForSeconds(6f);
        SetLaser(0.5f, 2);
        yield return new WaitForSeconds(0.2f);
        SetLaser(0.25f, 1);
        //attackPattern2LaserSpriteRenderer.color = new Color(attackPattern2LaserSpriteRenderer.color.r, attackPattern2LaserSpriteRenderer.color.g, attackPattern2LaserSpriteRenderer.color.b, 0.5f);
        yield return new WaitForSeconds(0.2f);
        SetLaser(0, 0);
        //attackPattern2LaserSpriteRenderer.color = new Color(attackPattern2LaserSpriteRenderer.color.r, attackPattern2LaserSpriteRenderer.color.g, attackPattern2LaserSpriteRenderer.color.b, 0f);
    }

    private void SetLaser(float colliderSize, int laserSprite)
    {
        if (colliderSize == 0)
        {
            attackPattern2LaserCollider.enabled = false;
        }
        else
        {
            attackPattern2LaserCollider.enabled = true;
            attackPattern2LaserCollider.size = new Vector2(colliderSize, 1f);
        }
        attackPattern2LaserSpriteRenderer.sprite = attackPattern2LaserSprites[laserSprite];
    }

    IEnumerator AttackPattern3()
    {
        Invincibility(true);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        stopAttacks = true;
        int minionAmmount = 5;
        attackPattern3EnemiesCount = minionAmmount;
        float randomXPos;
        float randomYPos;
        int randomEnemy;
        GameObject enemy;
        for (int i = 0; i < minionAmmount; i++)
        {
            randomXPos = Random.Range(-attackPattern3SpawnOffset.x, attackPattern3SpawnOffset.x);
            randomYPos = Random.Range(-attackPattern3SpawnOffset.y, attackPattern3SpawnOffset.y);
            randomEnemy = Random.Range(0, 2);
            switch (randomEnemy)
            {
                case 0:
                    enemy = Instantiate(attackPattern3BatEnemyPrefab, new Vector3(transform.position.x + randomXPos, transform.position.y + randomYPos, transform.position.z), Quaternion.identity);
                    enemy.GetComponent<BaseNPC>().onNPCDeath.AddListener(() => OnEnemyDeath());
                    break;

                case 1:
                    enemy = Instantiate(attackPattern3RamEnemyPrefab, new Vector3(transform.position.x + randomXPos, transform.position.y + randomYPos, transform.position.z), Quaternion.identity);
                    enemy.GetComponent<BaseNPC>().onNPCDeath.AddListener(() => OnEnemyDeath());
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnEnemyDeath()
    {
        attackPattern3EnemiesCount--;
        Debug.Log("KilledEnemy, enemies remaining: " + attackPattern3EnemiesCount);
        if(attackPattern3EnemiesCount <= 0)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            stopAttacks = false;
            Invincibility(false);
        }
    }
}
