using DG.Tweening;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnNPCDeath : UnityEvent { }
[Serializable]
public class OnNPCHit : UnityEvent<float> { }

public class BaseNPC : BaseEntity
{
    [SerializeField]
    protected AIPath path;
    [SerializeField]
    protected Rigidbody2D NPCRigidbody;

    public CustomSpriteAnimation spriteAnimation;
    [SerializeField]
    protected FacingSide facingSide;

    [SerializeField]
    protected EnemyParticleController enemyParticleController;

    public OnNPCDeath onNPCDeath;
    public OnNPCHit onNPCHit;

    private bool damageable = true;
    public bool isInvincible = false;

    #region Stats
    public float maxHealth;
    public float health;

    public float speed;
    public float baseSpeed;

    public float damage;
    #endregion

    #region Coroutines
    private Coroutine slowedCoroutine;
    #endregion

    public bool canAttack = false;

    public override void Start()
    {
        base.Start();
        if (path != null)
        {
            path.maxSpeed = baseSpeed;
        }

        if (TryGetComponent(out Drops drops))
        {
            onNPCDeath.AddListener(() =>
            {
                drops.DropCollectible();
            });
        }

        onNPCDeath.AddListener(() => destroyEvent.Invoke());
        onNPCDeath.AddListener(() => GameStateManager.instance.audioManager.PlaySoundEffect(onDeathAudioClip));
    }

    public virtual void Invincibility(bool state)
    {
        if (state)
        {
            spriteRenderer.material.SetFloat(InvincibilityID, 1);
            isInvincible = true;
        }
        else
        {
            spriteRenderer.material.SetFloat(InvincibilityID, 0);
            isInvincible = false;
        }
    }

    public void ApplyForce(Vector2 force)
    {
        if(NPCRigidbody != null)
            NPCRigidbody.AddRelativeForce(force);
    }

    protected void UpdateSpriteDirection(bool useRigidbody)
    {
        if (useRigidbody)
        {
            if (NPCRigidbody.velocity.x * (int)facingSide > 0f)
            {
                if (spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = false;
                }
            }
            if (NPCRigidbody.velocity.x * (int)facingSide < 0f)
            {
                if (!spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = true;
                }
            }
            return;
        }

        if(path != null)
        {
            if (path.desiredVelocity.x * (int)facingSide > 0f)
            {
                if (spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = false;
                }
            }
            if (path.desiredVelocity.x * (int)facingSide < 0f)
            {
                if (!spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = true;
                }
            }
            return;
        }

        float angle = MathExtensions.GetAngle(transform.position, GameManagerScript.instance.player.transform.position);
        Vector2 magnitude = MathExtensions.GetAngleMagnitude(angle, true);

        if(magnitude.x * (int)facingSide < 0)
        {
            if (spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
        if (magnitude.x * (int)facingSide > 0)
        {
            if (!spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (damageable && !isInvincible)
        {
            if(health - damage > 0)
            {
                GameStateManager.instance.audioManager.PlaySoundEffect(onHitAudioClip);
            }
            onNPCHit.Invoke(damage);
        }
    }

    protected IEnumerator DamageTimer()
    {
        spriteAnimation.stopAnimation = true;

        damageable = false;
        spriteRenderer.material.SetFloat(flashID, 1);
        //Debug.Log("SET TO 1");
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetFloat(flashID, 0);
        //Debug.Log("SET TO 0");
        damageable = true;

        spriteAnimation.stopAnimation = false;
    }

    public void Slowed()
    {
        if (slowedCoroutine != null)
        {
            StopCoroutine(slowedCoroutine);
        }

        slowedCoroutine = StartCoroutine(SlowTimer());
    }

    protected IEnumerator SlowTimer()
    {
        if(path != null)
        {
            isSlowed = true;
            speed = baseSpeed / 2;
            path.maxSpeed = speed;
            yield return new WaitForSeconds(2f);
            isSlowed = false;
            speed = baseSpeed;
            path.maxSpeed = speed;
        }
        else
        {
            isSlowed = true;
            yield return new WaitForSeconds(2f);
            isSlowed = false;
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}


public enum FacingSide
{
    Right = 1,
    Left = -1,
}
