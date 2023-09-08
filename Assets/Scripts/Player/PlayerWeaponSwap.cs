using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSwap : MonoBehaviour
{
    public WeaponType curerntWeaponType;

    public GameObject[] availableWeapons;
    public Sprite[] availableWeaponsImageSprites;

    public GameObject currentWeapon;
    public Sprite currentWeaponImageSprite;

    private int weaponIndex = 0;

    public byte unlockedWeapons = 1;

    public void OnWeaponSwitch(int increment)
    {
        weaponIndex += increment;
        if(weaponIndex > unlockedWeapons - 1)
        {
            weaponIndex = 0;
        }
        if(weaponIndex < 0 )
        {
            weaponIndex = unlockedWeapons - 1;
        }
        currentWeapon = availableWeapons[weaponIndex];
        curerntWeaponType = availableWeapons[weaponIndex].GetComponent<Projectile>().projectileType;
        currentWeaponImageSprite = availableWeaponsImageSprites[weaponIndex];
    }    
}

public enum WeaponType
{
    Basic,
    Heavy,
    Fire,
    Glue,
    Bomb
}
