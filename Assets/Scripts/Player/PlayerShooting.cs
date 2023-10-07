using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private Player player;
    private float cooldown = 0f;
    private float recallCooldown = 0.1f;
    [SerializeField]
    private float recallTime = 2f;
    [SerializeField]
    private bool isRecallHoldable = false;
    private float recallTimer;
    private bool alreadyRecalled = false;
    private bool SpawnedParticle = false;

    [SerializeField]
    private List<GameObject> ProjectileList = new List<GameObject>();

    public float forceMultiplier = 1f;

    [SerializeField]
    private GameObject recallParticle;
    private ParticleSystem particle;

    private void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        if (recallCooldown > 0)
        {
            recallCooldown -= Time.deltaTime;
        }

        if (!player.playerData.isRecallButtonHeld)
        {
            if(particle != null) 
            {
                player.remainingArrowScript.OnArrowStoppedRefreshing();
                particle.Stop();
                particle = null;
            }
            recallTimer = recallTime;
            alreadyRecalled = false;
            SpawnedParticle = false;
        }


        if (player.playerData.isRecallButtonHeld && recallCooldown <= 0)
        {
            if(player.playerData.currentArrowCount >= player.playerData.maxArrowCount)
            {
                return;
            }

            if(alreadyRecalled)
            {
                recallTimer = recallTime;
                if (isRecallHoldable)
                {
                    particle.Stop();
                    particle = null;
                    alreadyRecalled = false;
                    SpawnedParticle = false;
                }
                return;
            }

            if (recallTimer > 0)
            {
                if (!SpawnedParticle)
                {
                    particle = Instantiate(recallParticle, transform).GetComponent<ParticleSystem>();
                    SpawnedParticle = true;
                }
                recallTimer -= Time.deltaTime;

                player.remainingArrowScript.ArrowRefreshIcon(1 - recallTimer);

                return;
            }

            if(!alreadyRecalled)
            {
                alreadyRecalled = true;
                OnRecall();
                player.remainingArrowScript.OnArrowRefreshed();
                recallCooldown += 0.1f; 
                return;
            }
        }

        if (player.playerData.isShooting && cooldown <= 0)
        {
            if(player.playerData.currentArrowCount > 0)
            {
                OnShoot(out float attackSpeed);
                cooldown += attackSpeed;
            }
            return;
        }       
    }

    public void OnShoot(out float attackSpeed)
    {
        player.PlayAttackSoundEffect();

        arrowPrefab = player.playerWeaponSwap.currentWeapon;

        attackSpeed = arrowPrefab.GetComponent<Projectile>().projectileAttackSpeed;

        if (arrowPrefab.GetComponent<PlayerProjectile>().projectileType != WeaponType.Basic)
        {
            Debug.Log("SpecialArrowtip");
            //if(player.playerData.currentArrowtipsCount <= 0)
            //{
            //    Debug.LogWarning("No more arrowtips");
            //    return;
            //}
            player.arrowtipUsedEvent.Invoke();
        }
        else
        {
            Debug.Log("NormalArrow");
        }

        player.arrowUsedEvent.Invoke();

        GameObject projectile = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0,0,player.playerAim.angle));
        ProjectileList.Add(projectile);
        PlayerProjectile playerProjectile = projectile.GetComponent<PlayerProjectile>();

        playerProjectile.OnInstantiate(player.playerAim.angle, player.playerAim.distance, forceMultiplier);

        playerProjectile.destroyEvent.AddListener(() => ProjectileList.Remove(projectile));
        //playerProjectile.destroyEvent.AddListener(RenewArrow);
    }

    public void OnRecall()
    {
        if(ProjectileList.Count > 0)
        {
            ProjectileList[0].GetComponent<PlayerProjectile>().recallEvent.Invoke();
        }

        //for (int i = ProjectileList.Count - 1; i >= 0; i--)
        //{
        //    ProjectileList[i].GetComponent<PlayerProjectile>().recallEvent.Invoke();
        //}

        RenewArrow();
    }

    void RenewArrow()
    {
        player.arrowRenewEvent.Invoke();
    }

    //void RenewArrows()
    //{
    //    player.arrowsRenewEvent.Invoke();
    //}
}
