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
    public int mapXOffset;
    public int mapYOffset;
    public int mapID;
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

    public bool unlockedBow;
    public bool waterSpirit;
    public bool earthSpirit;
    public bool fireSpirit;
    #endregion

}

[System.Serializable]
public class NestedIntArray
{
    public int[] array = new int[100];
}

[System.Serializable]
public class NestedBoolArray
{
    public bool[] array = new bool[100];
}
