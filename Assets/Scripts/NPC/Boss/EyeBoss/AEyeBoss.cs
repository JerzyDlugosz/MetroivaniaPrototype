using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEyeBoss : EyeBoss
{
    [SerializeField]
    private SpriteRenderer laserSpriteRenderer;
    [SerializeField]
    private BoxCollider2D laserCollider;
    [SerializeField]
    private List<Sprite> laserSprites = new List<Sprite>();

    private bool stopMovement = false;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        attackCooldown = attackSpeed;
        stoppedEvent.AddListener(OnStop);

        ChooseNextMovementPoint();
    }

    private void Update()
    {
        if (isStopped)
            return;

        if (stopMovement)
            return;

        MovementAndRotationLogic();

        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0)
        {
            attackCooldown = attackSpeed;
            AttackLogic();
        }

    }
    private void AttackLogic()
    {
        attackCooldown += 6f / eyeComposite.remainingPartsModifier;
        AttackPattern1();
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    private void AttackPattern1()
    {
        stopMovement = true;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        Vector3 movePosition = GetRandomYMoveLocation();
        stopLookingAtPlayer = true;
        transform.DOLocalMove(movePosition, MathF.Min(Vector3.Distance(movePosition, transform.localPosition) / maxSpeed , 4f)).OnComplete(() =>
        {
            SetLaser(0f, 2);
            transform.DOShakePosition(1f, 0.2f).OnComplete(() => 
            {
                SetLaser(0.75f, 3);
                int rand = UnityEngine.Random.Range(0, 2);
                switch (rand)
                {
                    case 0:
                        movePosition = transform.localPosition - new Vector3(0, 4f * eyeComposite.remainingPartsModifier, 0f);
                        transform.DOLocalMove(movePosition, Vector3.Distance(movePosition, transform.localPosition) / maxSpeed).OnComplete(() =>
                        {
                            stopMovement = false;
                            SetLaser(0, 0);
                            stopLookingAtPlayer = false;
                        });
                        break;
                    case 1:
                        movePosition = transform.localPosition + new Vector3(0, 4f * eyeComposite.remainingPartsModifier, 0f);
                        transform.DOLocalMove(movePosition, Vector3.Distance(movePosition, transform.localPosition) / maxSpeed).OnComplete(() =>
                        {
                            stopMovement = false;
                            SetLaser(0, 0);
                            stopLookingAtPlayer = false;
                        });
                        break;
                }
            });
        });
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(damage);
        }
    }
}
