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

    private void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if (player.playerData.isShooting && cooldown <= 0)
        {
            OnShoot(out float attackSpeed);
            cooldown += attackSpeed;
        }
       
    }

    public void OnShoot(out float attackSpeed)
    {
        GameObject projectile = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0,0,player.playerAim.angle));
        projectile.GetComponent<Projectile>().OnInstantiate(player.playerAim.angle);
        attackSpeed = projectile.GetComponent<Projectile>().projectileAttackSpeed;
    }
}
