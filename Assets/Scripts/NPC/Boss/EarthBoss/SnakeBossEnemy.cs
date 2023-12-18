using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBossEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;
    [SerializeField]
    private bool basicSpriteRotation = true;

    private CompositeEnemy compositeEnemy;

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");
    public override void Start()
    {
        base.Start();
        compositeEnemy = GetComponentInParent<CompositeEnemy>();

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
        if (basicSpriteRotation)
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
    }
    private void OnHit(float damage)
    {
        compositeEnemy.compositeEnemyHealth -= damage;

        foreach (var item in compositeEnemy.enemyParts)
        {
            float scale = (compositeEnemy.compositeEnemyMaxHealth - compositeEnemy.compositeEnemyHealth) / compositeEnemy.compositeEnemyMaxHealth;
            item.bossEnemy.spriteRenderer.material.SetFloat(DamageScaleID, 1 + scale);
            item.moveSpeedMultiplier = 1 + (scale / 16);


        }

        if (compositeEnemy.compositeEnemyHealth <= 0f)
        {
            compositeEnemy.OnCompositeEnemyDeath();
            foreach (var item in compositeEnemy.enemyParts)
            {
                item.bossEnemy.onNPCDeath.Invoke();
            }
        }

        foreach (var item in compositeEnemy.enemyParts)
        {
            StartCoroutine(item.bossEnemy.DamageTimer());
        }
    }

    public void OnStop(bool state)
    {
        if (state)
            DOTween.Pause(gameObject.transform);
        else
            DOTween.Play(gameObject.transform);
    }

    public void OnDeath()
    {
        enemyParticleController.OnDeath();
        Destroy(gameObject);
    }

    public void PlayAttackSound()
    {
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }
}
