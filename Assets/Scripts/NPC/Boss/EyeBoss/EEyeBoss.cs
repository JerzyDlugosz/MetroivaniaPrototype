using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEyeBoss : EyeBoss
{
    [SerializeField]
    private GameObject AcidProjectile;

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

        MovementLogic();

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
        attackCooldown += 4f / eyeComposite.remainingPartsModifier;
        AttackPattern1();
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    private void AttackPattern1()
    {
        int projectileAmmount = (int)(5 * eyeComposite.remainingPartsModifier);
        float projectileSpread = 15 - (eyeComposite.remainingPartsModifier * eyeComposite.remainingPartsModifier);

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));

            GameObject projectile = Instantiate(AcidProjectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

        }
    }
}
