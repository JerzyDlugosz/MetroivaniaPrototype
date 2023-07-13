using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public int saveNumber;
    public bool[] unlockedMap = new bool[100*100];
    public int[] unlockedMinimapBackgrounds = new int[100*100];
    public int[] unlockedMinimapWalls = new int[100 * 100];
    public int[] unlockedMinimaDoors = new int[100 * 100];
    public int mapXOffset;
    public int mapYOffset;
    public int mapID;
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
