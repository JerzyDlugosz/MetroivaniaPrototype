using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretBoss : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool basicSpriteRotation = true;

    [SerializeField]
    private float attackSpeed = 1f;
    private float attackCooldown;

    [SerializeField]
    public BossData bossData;

    private bool stopCoroutines = false;
    private bool stopAttacks = false;

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");


    #region AttackPattern1Fields
    [SerializeField]
    private Transform water;
    [SerializeField]
    private SpriteRenderer laserSpriteRenderer;
    [SerializeField]
    private BoxCollider2D laserCollider;
    [SerializeField]
    private List<Sprite> laserSprites = new List<Sprite>();
    #endregion

    #region AttackPattern2Fields
    [SerializeField]
    private GameObject secretBossLaserEyePrefab;
    [SerializeField]
    private List<GameObject> secretBossLaserEyeList;
    [SerializeField]
    private List<GameObject> projectileList;
    [SerializeField]
    private List<Transform> eyePositions;
    #endregion

    #region AttackPattern3Fields
    [SerializeField]
    private GameObject Attack3Projectile;
    #endregion

    #region AttackPattern4Fields
    [SerializeField]
    private List<LaserTrapController> laserTrapControllers;
    #endregion

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        attackCooldown = 4f;
        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }

        UpdateSpriteDirection(false);
        spriteAnimation.UpdateAnimationFrame();

        if (!canAttack || stopAttacks)
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
        float scale = (maxHealth - health) / maxHealth;
        spriteRenderer.material.SetFloat(DamageScaleID, 1 + scale);
        attackSpeed = 1 + scale / 2;
        if (health <= 0f)
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

    public void FadeIn()
    {
        spriteRenderer.DOFade(1, 2f);
        GameManagerScript.instance.cameraHolder.DOShakePosition(2f, 1.5f);
    }

    public void SetAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }

    public void OnDeath()
    {
        ClearAllAttacks();
        GameManagerScript.instance.player.progressTracker.AddBoss(bossData);
        enemyParticleController.OnDeath();
        Destroy(gameObject);
    }

    private void ClearAllAttacks()
    {
        foreach (var item in laserTrapControllers)
        {
            item.LaserState(false);
        }
        foreach (var item in secretBossLaserEyeList)
        {
            Destroy(item);
        }
        secretBossLaserEyeList.Clear();

        foreach (var item in projectileList)
        {
            Destroy(item);
        }
        projectileList.Clear();
        water.GetComponentInChildren<Thorns>().isEnabled = false;
        SetLaser(0, 0);
        water.gameObject.SetActive(false);


    }

    private void AttackLogic()
    {
        int rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                attackCooldown += 7f / attackSpeed;
                StartCoroutine(AttackPattern1());
                break;
            case 1:
                attackCooldown += 16f / attackSpeed;
                StartCoroutine(AttackPattern2());
                break;
            case 2:
                attackCooldown += 4f / attackSpeed;
                StartCoroutine(AttackPattern3());
                break;
            case 3:
                attackCooldown += 6f / attackSpeed;
                StartCoroutine(AttackPattern4());
                break;
        }
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    IEnumerator AttackPattern1()
    {
        //int projectileAmmount = 5;
        //float projectileSpread = 15;

        //for (int i = 0; i < projectileAmmount; i++)
        //{
        //    float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

        //    Vector3 position = new Vector3(transform.position.x, transform.position.y, -4f);

        //    Transform playerTransform = GameManagerScript.instance.player.transform;

        //    float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
        //    Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

        //    GameObject projectile = Instantiate(attackPattern1ProjectilePrefab, position, targetRotation);

        //    projectile.GetComponent<Projectile>().OnInstantiate();

        //    yield return new WaitForSeconds(0.5f);
        //}

        water.DOLocalMoveY(0, 5f / attackSpeed);
        yield return new WaitForSeconds(4f / attackSpeed);
        SetLaser(0f, 2);
        yield return new WaitForSeconds(1f / attackSpeed);
        water.GetComponentInChildren<Thorns>().isEnabled = true;
        SetLaser(0.75f, 3);
        yield return new WaitForSeconds(2f / attackSpeed);
        SetLaser(0, 0);
        water.GetComponentInChildren<Thorns>().isEnabled = false;
        water.DOLocalMoveY(-7, 5f / attackSpeed);
    }

    private void SetLaser(float colliderSize, int laserSprite)
    {
        if (colliderSize == 0)
        {
            laserCollider.enabled = false;
        }
        else
        {
            laserCollider.enabled = true;
            laserCollider.size = new Vector2(colliderSize, 1f);
        }
        laserSpriteRenderer.sprite = laserSprites[laserSprite];
    }

    IEnumerator AttackPattern2()
    {
        for (int i = 0; i < 4; i++)
        {
            int random = Random.Range(0, 5);
            bool rotation;
            if (random <= 2)
                rotation = true;
            else
                rotation = false;
            SecretBossLaserEyes secretBossLaserEyes = Instantiate(secretBossLaserEyePrefab, transform.position, secretBossLaserEyePrefab.transform.rotation).GetComponent<SecretBossLaserEyes>();
            secretBossLaserEyeList.Add(secretBossLaserEyes.gameObject);
            secretBossLaserEyes.OnSecretBossEyeDestroyEvent.AddListener(() =>
            {
                secretBossLaserEyeList.Remove(secretBossLaserEyes.gameObject);
            });
            secretBossLaserEyes.EyeAttack(eyePositions[random].position, rotation);
            yield return new WaitForSeconds(4f / attackSpeed);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(damage);
        }
    }

    IEnumerator AttackPattern3()
    {
        projectileList.Clear();
        int projectileAmmount = 5;
        float projectileSpread = 15f;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(Attack3Projectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

            projectileList.Add(projectile.gameObject);
        }

        yield return new WaitForSeconds(1f / attackSpeed);

        projectileAmmount = 4;
        projectileSpread = 15f;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(Attack3Projectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();
            projectileList.Add(projectile.gameObject);
        }

        yield return new WaitForSeconds(1f / attackSpeed);

        projectileAmmount = 5;
        projectileSpread = 15f;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(Attack3Projectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();
            projectileList.Add(projectile.gameObject);
        }
    }

    IEnumerator AttackPattern4()
    {
        foreach (var item in laserTrapControllers)
        {
            item.LaserState(true);
        }
        yield return new WaitForSeconds(7f / attackSpeed);
        foreach (var item in laserTrapControllers)
        {
            item.LaserState(false);
        }
    }
}
