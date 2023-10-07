using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Collectible
{
    public int collectibleId;
    public int collectiblePosX;
    public int collectiblePosY;
    public Sprite sprite;
    public CollectibleType collectibleType;
}
