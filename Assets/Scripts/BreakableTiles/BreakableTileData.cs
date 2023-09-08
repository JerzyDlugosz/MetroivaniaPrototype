using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableTileData : MonoBehaviour
{
    public MaterialType material;
}

public enum MaterialType
{
    Dirt,
    Wood,
    Stone
}