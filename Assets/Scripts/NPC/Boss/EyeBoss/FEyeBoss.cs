using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEyeBoss : EyeBoss
{
    [SerializeField]
    private GameObject FireProjectile;

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
        int projectileNumber = (int)( 8 * eyeComposite.remainingPartsModifier);
        Vector3 projectilePos = new Vector3(transform.position.x, transform.position.y - projectileNumber/2, transform.position.z);
        for (int i = 0; i < projectileNumber; i++)
        {
            projectilePos.Set(projectilePos.x, projectilePos.y + 1, projectilePos.z);
            GameObject projectile = Instantiate(FireProjectile, projectilePos, Quaternion.identity);
            projectile.GetComponent<Projectile>().OnInstantiate();
        }
        
    }
}
