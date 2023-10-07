using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamBatEnemy : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;
    [SerializeField]
    private bool isUsingRigidbody = false;

    [SerializeField]
    private float chargeAttackSpeed;
    private float chargeCooldown;
    [SerializeField]
    private float chargeForce;

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

        if (path.reachedEndOfPath)
        {
            if (chargeCooldown <= 0)
            {
                AttackPattern1();
                GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
                chargeCooldown = chargeAttackSpeed;
            }
        }
        if (chargeCooldown > 0)
        {
            chargeCooldown -= Time.deltaTime;
        }

    }

    public void OnStop(bool state)
    {
        path.canMove = !state;
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

    private void AttackPattern1()
    {
        stoppedEvent.Invoke(true);
        isStopped = true;
        float angle = MathExtensions.GetAngle(transform.position, GameManagerScript.instance.player.transform.position);
        Vector2 magnitude = MathExtensions.GetAngleMagnitude(angle, false);
        Debug.Log(magnitude);
        NPCRigidbody.AddForce(chargeForce * magnitude);

        float distance = Vector2.Distance(transform.position, GameManagerScript.instance.player.transform.position);
        StartCoroutine(delay(distance / 4));

    }

    private IEnumerator delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        stoppedEvent.Invoke(false);
        isStopped = false;
        NPCRigidbody.velocity = Vector2.zero;
    }
}
