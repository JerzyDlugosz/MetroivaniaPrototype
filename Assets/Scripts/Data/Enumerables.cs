using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumerables : MonoBehaviour
{
 
}

public enum PlayerStates
{
    Idle,
    Walking,
    Running,
    InAir,
    Jumping,
    InWater,
    InLava
}

public enum Zone
{
    Zone1,
    Zone2,
    Zone3,
    Menu
}

public enum CollectibleSpriteIDs
{
    FireArrow = 0,
    HeavyArrow = 1,
    BombArrow = 2,
    GlueArrow = 3,
    BummerangArrow = 4,
    WaterSpirit = 5,
    EarthSpirit = 6,
    FireSpirit = 7,
    AirSpirit = 8,
    NoSprite = -1,
}
