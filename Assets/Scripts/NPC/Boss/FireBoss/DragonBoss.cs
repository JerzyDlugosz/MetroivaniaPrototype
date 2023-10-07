using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class DragonBoss : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;
    [SerializeField]
    private bool basicSpriteRotation = true;

    [SerializeField]
    private DragonComposite dragonComposite;


    [SerializeField]
    private float attackSpeed = 4f;
    private float attackCooldown;

    [SerializeField]
    public BossData bossData;
    [SerializeField]
    private Sprite baseSprite;
    [SerializeField]
    private Transform baseWaypoint;
    [SerializeField]
    private Transform groundPosition;

    private bool stopCoroutines = false;
    [SerializeField]
    private bool isPartOfBoss = false;


    #region AttackPattern1Fields
    [SerializeField]
    private List<Sprite> attackPattern1Sprites;
    [SerializeField]
    private List<Transform> attackPattern1Waypoints;
    [SerializeField]
    private GameObject attackPattern1Projectile;
    [SerializeField]
    private GameObject attackPattern1Button;
    [SerializeField]
    private GameObject attackPattern1PlatformBlock;
    private Coroutine blockadeCoroutine;
    #endregion

    #region AttackPattern2Fields
    [SerializeField]
    private GameObject attackPattern2SolidProjectile;
    [SerializeField]
    private GameObject attackPattern2CrackedProjectile;
    [SerializeField]
    private GameObject attackPattern2RockLine;
    #endregion

    #region AttackPattern3Fields
    [SerializeField]
    private GameObject attackPattern3ShockWave;
    [SerializeField]
    private Transform handSlamPosition;
    #endregion

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

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
        if(isPartOfBoss)
        {
            return;
        }
        if (isStopped)
        {
            return;
        }
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;


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

        if (attackCooldown <= 0)
        {
            attackCooldown = attackSpeed;
            AttackLogic();
        }

    }
    private void OnHit(float damage)
    {
        dragonComposite.TakeDamage(damage);

        StartCoroutine(DamageTimer());
    }

    public void OnStop(bool state)
    {
        stopCoroutines = state;

        if(state)
        {
            transform.DOPause();
        }

        if(!state)
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
        int rand = Random.Range(0, 3);
        float timeBetweenMove;

        switch (rand)
        {
            case 0:
                timeBetweenMove = 1f;
                attackCooldown += 6f;
                AttackPattern1(timeBetweenMove);
                StartCoroutine(AttackPattern1BreathAttack(timeBetweenMove));
            break;

            case 1:
                timeBetweenMove = 2f;
                attackCooldown += 8f;
                StartCoroutine(AttackPattern2(timeBetweenMove));
                break;

            case 2:
                timeBetweenMove = 0.05f;
                attackCooldown += 8f;
                StartCoroutine(AttackPattern3(timeBetweenMove));
                break;

        }
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    private void AttackPattern1(float timeBetweenMove)
    {
        attackPattern1Button.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        spriteAnimation.stopAnimation = true;
        transform.DOLocalMove(attackPattern1Waypoints[0].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
        {
            spriteRenderer.sprite = attackPattern1Sprites[0];
            transform.DOLocalMove(attackPattern1Waypoints[1].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
            {
                spriteRenderer.sprite = attackPattern1Sprites[1];
                transform.DOLocalMove(attackPattern1Waypoints[2].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
                {
                    spriteRenderer.sprite = attackPattern1Sprites[1];
                    transform.DOLocalMove(attackPattern1Waypoints[3].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        spriteRenderer.sprite = attackPattern1Sprites[2];
                        transform.DOLocalMove(attackPattern1Waypoints[4].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            spriteRenderer.sprite = attackPattern1Sprites[1];
                            transform.DOLocalMove(baseWaypoint.localPosition, timeBetweenMove).SetEase(Ease.Linear);
                            spriteAnimation.stopAnimation = false;
                            attackPattern1Button.transform.DOLocalMoveX(1, 0.5f).SetEase(Ease.Linear);
                            //if(blockadeCoroutine != null)
                            //{
                            //    HideBlockade();
                            //}
                        });
                    });
                });
            });
        });
    }

    #region AttackPattern1
    public void ShowBlocade()
    {
        attackPattern1PlatformBlock.SetActive(true);
        blockadeCoroutine = StartCoroutine(BlockadeTimer(5f));
    }

    private void HideBlockade()
    {

        StopCoroutine(blockadeCoroutine);
        attackPattern1PlatformBlock.SetActive(false);
    }

    IEnumerator BlockadeTimer(float time)
    {
        float t = 0;
        do
        {
            t += Time.deltaTime;
            Color.Lerp(attackPattern1PlatformBlock.GetComponent<Tilemap>().color, new Color(1, 1, 1, 0), t);
            yield return new WaitForFixedUpdate();
        } while (t < 1);

        yield return new WaitForSeconds(time);

        t = 0;

        do
        {
            t += Time.deltaTime;
            Color.Lerp(attackPattern1PlatformBlock.GetComponent<Tilemap>().color, new Color(1, 1, 1, 1), t);
            yield return new WaitForFixedUpdate();
        } while (t < 1);
        attackPattern1PlatformBlock.SetActive(false);
    }

    IEnumerator AttackPattern1BreathAttack(float timeBetweenMove)
    {
        yield return new WaitForSeconds(timeBetweenMove);

        Vector3 projectilePos = new Vector3(baseWaypoint.position.x - 22, baseWaypoint.position.y - 6);

        int rand = Random.Range(0, 8);

        for (int j = 0; j < 8; j++)
        {
            if (j == rand)
            {
                projectilePos.Set(projectilePos.x + 6, projectilePos.y, projectilePos.z);
                yield return new WaitForSeconds(timeBetweenMove / 2);
                continue;
            }

            for (int i = 0; i < 6; i++)
            {
                projectilePos.Set(projectilePos.x + 1, projectilePos.y, projectilePos.z);
                yield return new WaitUntil(() => !stopCoroutines);
                GameObject projectile = Instantiate(attackPattern1Projectile, projectilePos, Quaternion.identity);
                projectile.GetComponent<Projectile>().OnInstantiate();
                yield return new WaitForSeconds(timeBetweenMove / 12);
            }
        }
    }
    #endregion

    #region AttackPattern2

    IEnumerator AttackPattern2(float timeBetweenMove)
    {
        spriteRenderer.DOFade(0.3f, 1f);
        dragonComposite.dragonParts[1].spriteRenderer.DOFade(0.3f, 1f);
        dragonComposite.dragonParts[2].spriteRenderer.DOFade(0.3f, 1f);
        int rand = Random.Range(-24, 20);

        for (int j = 0; j < 4; j++)
        {
            Debug.Log($"rand = {rand}");
            int rand2 = Random.Range(-1, 2);
            rand += rand2 * 6;

            if (rand <= -20)
                rand = -20;
            
            if (rand >= 20)
                rand = 20;
            
            yield return new WaitUntil(() => !stopCoroutines);
            GameObject rockLine = Instantiate(attackPattern2RockLine, new Vector3(baseWaypoint.position.x, baseWaypoint.position.y), Quaternion.identity);
            for (int i = -24; i < 24; i++)
            {
                Vector3 projectilePos;
                GameObject projectile;
                if (i >= rand && i < rand + 4)
                {
                    projectilePos = new Vector3(baseWaypoint.position.x + i, baseWaypoint.position.y);
                    projectile = Instantiate(attackPattern2CrackedProjectile, projectilePos, Quaternion.identity);
                    rockLine.GetComponent<PRockLine>().crackedRocks.Add(projectile);
                    projectile.GetComponent<Projectile>().OnInstantiate();

                    projectile.transform.SetParent(rockLine.transform);
                    rockLine.GetComponent<PRockLine>().allRocks.Add(projectile);
                    projectile.GetComponent<Projectile>().destroyEvent.AddListener(() => rockLine.GetComponent<PRockLine>().allRocks.Remove(projectile));
                    continue;
                }

                projectilePos = new Vector3(baseWaypoint.position.x + i, baseWaypoint.position.y);
                projectile = Instantiate(attackPattern2SolidProjectile, projectilePos, Quaternion.identity);
                projectile.GetComponent<Projectile>().OnInstantiate();

                projectile.transform.SetParent(rockLine.transform);
                rockLine.GetComponent<PRockLine>().allRocks.Add(projectile);
                projectile.GetComponent<Projectile>().destroyEvent.AddListener(() => rockLine.GetComponent<PRockLine>().allRocks.Remove(projectile));
            }


            //One Line Projectile

            //projectilePos = new Vector3(baseWaypoint.position.x, baseWaypoint.position.y);
            //projectile = Instantiate(attackPattern2RockLine, projectilePos, Quaternion.identity);

            //for (int i = 0; i < projectile.transform.childCount; i++)
            //{
            //    projectile.transform.GetChild(i).AddComponent<BreakableProjectile>();
            //    projectile.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.blue;
            //}

            PRockLine pRockLine = rockLine.GetComponent<PRockLine>();


            foreach (var item in pRockLine.crackedRocks)
            {
                item.GetComponent<PCrackedRock>().OnSingleRockHitEvent.AddListener(() =>
                {
                    for (int i = 0; i < pRockLine.crackedRocks.Count; i++)
                    {
                        pRockLine.crackedRocks[i].GetComponent<PCrackedRock>().destroyEvent.Invoke();
                    }
                });
            }
            yield return new WaitForSeconds(timeBetweenMove);
        }

        spriteRenderer.DOFade(1f, 1f);
        dragonComposite.dragonParts[1].spriteRenderer.DOFade(1f, 1f);
        dragonComposite.dragonParts[2].spriteRenderer.DOFade(1f, 1f);

    }
    //private void AttackPattern2()
    //{
    //    spriteRenderer.DOFade(0.3f, 1f);

    //    spriteRenderer.DOFade(1f, 1f);
    //}
    #endregion

    #region AttackPattern3

    IEnumerator AttackPattern3(float timeBetweenMove)
    {

        Vector3 projectilePos = new Vector3(-24,0,0);
        GameObject projectile;


        dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[1];
        dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[3];
        dragonComposite.dragonParts[1].transform.DOLocalMoveY(handSlamPosition.localPosition.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
        {
            dragonComposite.dragonParts[1].transform.DOLocalMoveY(20, 1f).SetEase(Ease.InSine);
            dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[0];
        });

        dragonComposite.dragonParts[2].transform.DOLocalMoveY(handSlamPosition.localPosition.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
        {
            dragonComposite.dragonParts[2].transform.DOLocalMoveY(20, 1f).SetEase(Ease.InSine);
            dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[2];
        });

        for (int i = 0; i < 24; i++)
        {
            yield return new WaitUntil(() => !stopCoroutines);

            projectilePos = new Vector3(projectilePos.x + 2, projectilePos.y);
            Vector3 projectilePos2 = new Vector3(groundPosition.position.x + projectilePos.x, groundPosition.position.y + 1.5f, projectilePos.z);
            Debug.Log(projectilePos2);
            projectile = Instantiate(attackPattern3ShockWave, projectilePos2, Quaternion.identity);
            yield return new WaitForSeconds(timeBetweenMove);
        }
    }
    #endregion
}
