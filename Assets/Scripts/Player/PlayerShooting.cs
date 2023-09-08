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
    private List<GameObject> ProjectileList = new List<GameObject>();

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

        if (player.playerData.isRecallButtonHeld && recallCooldown <= 0)
        {
            OnRecall();
            recallCooldown += 0.1f;
            return;
        }

        if (player.playerData.isShooting && cooldown <= 0)
        {
            if(player.playerData.currentArrowCount > 0)
            {
                OnShoot(out float attackSpeed);
                cooldown += attackSpeed;
                player.arrowUsedEvent.Invoke();
            }
            return;
        }       
    }

    public void OnShoot(out float attackSpeed)
    {
        arrowPrefab = player.playerWeaponSwap.currentWeapon;

        GameObject projectile = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0,0,player.playerAim.angle));
        ProjectileList.Add(projectile);
        PlayerProjectile playerProjectile = projectile.GetComponent<PlayerProjectile>();

        playerProjectile.OnInstantiate(player.playerAim.angle);

        attackSpeed = projectile.GetComponent<Projectile>().projectileAttackSpeed;

        playerProjectile.destroyEvent.AddListener(() => ProjectileList.Remove(projectile));
        playerProjectile.destroyEvent.AddListener(RenewArrow);
    }

    public void OnRecall()
    {
        for (int i = 0; i < ProjectileList.Count; i++)
        {
            ProjectileList[i].GetComponent<PlayerProjectile>().recallEvent.Invoke();
        }
    }

    void RenewArrow()
    {
        player.arrowRenewEvent.Invoke();
    }
}
