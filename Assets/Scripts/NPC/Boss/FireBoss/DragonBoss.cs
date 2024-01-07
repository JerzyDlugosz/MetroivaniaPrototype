using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private DragonComposite dragonComposite;


    [SerializeField]
    private float attackSpeed = 1f;
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

    private int previousAttack = -1;

    private int fallingRockAttackCooldown = 2;


    private List<SpriteRenderer> handSprites = new List<SpriteRenderer>();
    private List<SpriteRenderer> AllSprites = new List<SpriteRenderer>();


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

        attackCooldown = 2f;
        stoppedEvent.AddListener(OnStop);

        handSprites.Add(dragonComposite.dragonParts[1].spriteRenderer);
        handSprites.Add(dragonComposite.dragonParts[2].spriteRenderer);

        AllSprites.Add(spriteRenderer);
        AllSprites.Add(handSprites[0]);
        AllSprites.Add(handSprites[1]);
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
            AttackLogic();
        }

    }
    private void OnHit(float damage)
    {
        float scale = dragonComposite.TakeDamage(damage);
        attackSpeed = 1 + scale / 2;
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
        //Destroy(gameObject);
    }

    private void AttackLogic()
    {
        int rand = Random.Range(0, 3);
        float timeBetweenMove;

        if (previousAttack == rand)
        {
            rand -= 1;
            if (rand < 0)
                rand = 2;
        }
        previousAttack = rand;

        if(rand == 1)
        {
            if(fallingRockAttackCooldown > 0)
            {
                rand = Random.Range(0, 2);
                if (rand == 1)
                    rand = 2;
            }

        }
        if(fallingRockAttackCooldown > 0)
            fallingRockAttackCooldown--;

        switch (rand)
        {
            //Flame attack
            case 0:
                timeBetweenMove = 1f;
                attackCooldown += 8f / attackSpeed;
                AttackPattern1(timeBetweenMove);
                StartCoroutine(AttackPattern1BreathAttack(timeBetweenMove));
                break;

            //Falling Rocks attack    
            case 1:
                fallingRockAttackCooldown = 2;
                timeBetweenMove = 2f;
                attackCooldown += 9f;
                StartCoroutine(AttackPattern2(timeBetweenMove));
                break;

            //Shockwave attack
            case 2:
                timeBetweenMove = 0.05f;
                attackCooldown += 5f / attackSpeed;
                StartCoroutine(AttackPattern3(timeBetweenMove));
                break;

        }
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    private void Hide(List<SpriteRenderer> dragonPartsSpriteRenderers, float hideAmmount, float scaleAmmount)
    {
        foreach (var sprite in dragonPartsSpriteRenderers)
        {
            sprite.DOFade(hideAmmount, 1f);
            sprite.transform.DOScale(scaleAmmount, 1f);
        }
    }

    private void AttackPattern1(float timeBetweenMove)
    {

        Hide(handSprites, 0.6f, 1f);
        spriteAnimation.stopAnimation = true;
        transform.DOLocalRotate(new Vector3(0,0,-40f), 0.75f / attackSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOLocalRotate(new Vector3(0, 0, 40f), 4.5f / attackSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                transform.DOLocalRotate(new Vector3(0, 0, 0), 0.75f / attackSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    spriteAnimation.stopAnimation = false;
                    Hide(handSprites, 1f, 1f);
                });
            });
        });


        //attackPattern1Button.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        //transform.DOLocalMove(attackPattern1Waypoints[0].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
        //{
        //    spriteRenderer.sprite = attackPattern1Sprites[0];
        //    transform.DOLocalMove(attackPattern1Waypoints[1].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        spriteRenderer.sprite = attackPattern1Sprites[1];
        //        transform.DOLocalMove(attackPattern1Waypoints[2].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
        //        {
        //            spriteRenderer.sprite = attackPattern1Sprites[1];
        //            transform.DOLocalMove(attackPattern1Waypoints[3].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
        //            {
        //                spriteRenderer.sprite = attackPattern1Sprites[2];
        //                transform.DOLocalMove(attackPattern1Waypoints[4].localPosition, timeBetweenMove).SetEase(Ease.Linear).OnComplete(() =>
        //                {
        //                    spriteRenderer.sprite = attackPattern1Sprites[1];
        //                    transform.DOLocalMove(baseWaypoint.localPosition, timeBetweenMove).SetEase(Ease.Linear);
        //                    spriteAnimation.stopAnimation = false;
        //                    //attackPattern1Button.transform.DOLocalMoveX(1, 0.5f).SetEase(Ease.Linear);

        //                    Hide(handSprites, 1f, 1f);

        //                });
        //            });
        //        });
        //    });
        //});
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

        int rand = Random.Range(1, 7);

        for (int j = 0; j < 8; j++)
        {
            if (j == rand)
            {
                projectilePos.Set(projectilePos.x + 6, projectilePos.y, projectilePos.z);
                yield return new WaitForSeconds(timeBetweenMove / 2 / attackSpeed);
                continue;
            }

            for (int i = 0; i < 6; i++)
            {
                projectilePos.Set(projectilePos.x + 1, projectilePos.y, projectilePos.z);
                yield return new WaitUntil(() => !stopCoroutines);
                GameObject projectile = Instantiate(attackPattern1Projectile, projectilePos, Quaternion.identity);
                projectile.GetComponent<Projectile>().OnInstantiate(1 * attackSpeed);
                projectile.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y, -2f);
                yield return new WaitForSeconds(timeBetweenMove / 12 / attackSpeed);
            }
        }
    }
    #endregion

    #region AttackPattern2

    IEnumerator AttackPattern2(float timeBetweenMove)
    {
        Invincibility(true);

        GetComponent<Collider2D>().enabled = false;

        //dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[1];
        //dragonComposite.dragonParts[1].transform.DOLocalMoveY(handSlamPosition.localPosition.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
        //{
        //    dragonComposite.dragonParts[1].transform.DOLocalMoveY(20, 1f).SetEase(Ease.InSine);
        //    dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[0];
        //});

        //dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[3];
        //dragonComposite.dragonParts[2].transform.DOLocalMoveY(handSlamPosition.localPosition.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
        //{
        //    dragonComposite.dragonParts[2].transform.DOLocalMoveY(20, 1f).SetEase(Ease.InSine);
        //    dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[2];
        //    GameManagerScript.instance.cameraMovement.transform.DOShakePosition(1f, 1f);
        //});


        dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[1];
        dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[4];
        yield return new WaitForSeconds(0.2f);

        dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[2];
        dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[5];
        yield return new WaitForSeconds(0.2f);

        dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[0];
        dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[3];
        yield return new WaitForSeconds(0.1f);
        GameManagerScript.instance.cameraHolder.DOShakePosition(1f, 1f);
        Hide(AllSprites, 0.6f, 1f);

        Vector2Int PlayerToCenterPos = new Vector2Int((int)(GameManagerScript.instance.player.transform.position.x - groundPosition.position.x), (int)(GameManagerScript.instance.player.transform.position.y - groundPosition.position.y));

        //int startingCrackedWallPos = Random.Range(-24, 20);
        int startingCrackedWallPos = Random.Range(-PlayerToCenterPos.x, PlayerToCenterPos.x + 4);

        //Debug.LogWarning("PlayerPos: " + PlayerToCenterPos);

        for (int j = 0; j < 4; j++)
        {
           // Debug.Log($"rand = {startingCrackedWallPos}");
            int CrackedWallMoveDirection = Random.Range(-1, 2);
            startingCrackedWallPos += CrackedWallMoveDirection * 6;

            if (startingCrackedWallPos <= -20)
                startingCrackedWallPos = -20;
            
            if (startingCrackedWallPos >= 20)
                startingCrackedWallPos = 20;
            
            yield return new WaitUntil(() => !stopCoroutines);
            GameObject rockLine = Instantiate(attackPattern2RockLine, new Vector3(baseWaypoint.position.x, baseWaypoint.position.y), Quaternion.identity);
            for (int i = -24; i < 24; i++)
            {
                Vector3 projectilePos;
                GameObject projectile;
                if (i >= startingCrackedWallPos && i < startingCrackedWallPos + 4)
                {
                    projectilePos = new Vector3(baseWaypoint.position.x + i, baseWaypoint.position.y);
                    projectile = Instantiate(attackPattern2CrackedProjectile, projectilePos, Quaternion.identity);
                    rockLine.GetComponent<PRockLine>().crackedRocks.Add(projectile);
                    projectile.GetComponent<Projectile>().OnInstantiate();

                    projectile.transform.SetParent(rockLine.transform);
                    rockLine.GetComponent<PRockLine>().allRocks.Add(projectile);
                    projectile.GetComponent<Projectile>().destroyEvent.AddListener(() => rockLine.GetComponent<PRockLine>().allRocks.Remove(projectile));
                    projectile.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y, -2f);
                    continue;
                }

                projectilePos = new Vector3(baseWaypoint.position.x + i, baseWaypoint.position.y);
                projectile = Instantiate(attackPattern2SolidProjectile, projectilePos, Quaternion.identity);
                projectile.GetComponent<Projectile>().OnInstantiate();

                projectile.transform.SetParent(rockLine.transform);
                rockLine.GetComponent<PRockLine>().allRocks.Add(projectile);
                projectile.GetComponent<Projectile>().destroyEvent.AddListener(() => rockLine.GetComponent<PRockLine>().allRocks.Remove(projectile));
                projectile.transform.position = new Vector3(projectile.transform.position.x, projectile.transform.position.y, -2f);
            }

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

        Hide(AllSprites, 1f, 1f);

        Invincibility(false);
        GetComponent<Collider2D>().enabled = true;
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

        int shockwaveDirection = Random.Range(0, 2);

        int shockwaveStartingPos = -48 * shockwaveDirection;


        Vector3 projectilePos = new Vector3(24 + shockwaveStartingPos, 0, 0);
        GameObject projectile;


        if (shockwaveDirection == 1)
        {
            dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[1];
            yield return new WaitForSeconds(0.2f);

            dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[2];
            yield return new WaitForSeconds(0.2f);

            dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[0];
        }
        else
        {
            dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[4];
            yield return new WaitForSeconds(0.2f);

            dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[5];
            yield return new WaitForSeconds(0.2f);

            dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[3];
        }

        GameManagerScript.instance.cameraHolder.DOShakePosition(1f, 1f);

        //if (shockwaveDirection == 1)
        //{
        //    dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[1];
        //    dragonComposite.dragonParts[1].transform.DOLocalMoveY(handSlamPosition.localPosition.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
        //    {
        //        dragonComposite.dragonParts[1].transform.DOLocalMoveY(20, 1f).SetEase(Ease.InSine);
        //        dragonComposite.dragonParts[1].spriteRenderer.sprite = dragonComposite.handSprites[0];
        //        GameManagerScript.instance.cameraMovement.transform.DOShakePosition(1f, 1f);
        //    });
        //}
        //else
        //{
        //    dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[3];
        //    dragonComposite.dragonParts[2].transform.DOLocalMoveY(handSlamPosition.localPosition.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
        //    {
        //        dragonComposite.dragonParts[2].transform.DOLocalMoveY(20, 1f).SetEase(Ease.InSine);
        //        dragonComposite.dragonParts[2].spriteRenderer.sprite = dragonComposite.handSprites[2];
        //        GameManagerScript.instance.cameraMovement.transform.DOShakePosition(1f, 1f);
        //    });
        //}

        for (int i = 0; i < 24; i++)
        {
            yield return new WaitUntil(() => !stopCoroutines);

            if (shockwaveDirection == 1)
            {
                projectilePos = new Vector3(projectilePos.x + 2, projectilePos.y);
            }
            else
            {
                projectilePos = new Vector3(projectilePos.x -2, projectilePos.y);
            }
            Vector3 projectilePos2 = new Vector3(groundPosition.position.x + projectilePos.x, groundPosition.position.y + 1.5f, projectilePos.z);
            projectile = Instantiate(attackPattern3ShockWave, projectilePos2, Quaternion.identity);
            yield return new WaitForSeconds(timeBetweenMove);
        }

    }
    #endregion
}
