using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public int saveNumber;

    #region MapAndMinimap
    public bool[] unlockedMap = new bool[100*100];
    public int[] unlockedMinimapBackgrounds = new int[100*100];
    public int[] unlockedMinimapWalls = new int[100 * 100];
    public int[] unlockedMinimaDoors = new int[100 * 100];
    #endregion

    #region SaveLocation
    public int mapXOffset;
    public int mapYOffset;
    public int mapID;
    public Zone zone;
    #endregion

    #region BossAndCollectibleProgress
    public List<BossData> bossesSlayed;
    public List<Collectible> collectibles;
    #endregion

    #region Stats
    public int maxHealth;
    public float currentHealth;
    public byte maxArrowCount;
    public byte currentArrowCount;
    public byte unlockedWeapons;
    public float reloadSpeedModifier;
    public float damageModifier;

    public bool unlockedBow;
    public bool waterSpirit;
    public bool earthSpirit;
    public bool fireSpirit;
    public bool airSpirit;
    #endregion

    #region Leaderboard
    public float timePlayed;
    #endregion
}