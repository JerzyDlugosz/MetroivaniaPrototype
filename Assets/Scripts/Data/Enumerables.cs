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
    Menu,
    SpecialZone,
    SpecialBoss,
    TrueEnd
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
    BasicArrow = 9,
}

public enum SpecialRoom
{
    NormalRoom = 0,
    SavePoint = 1,
    BoosRoom = 2,
}

public enum InputMode
{
    Game = 0,
    Menu = 1,
    None = 2,
}
