using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/CollectibleList")]
public class CollectibleList : ScriptableObject
{
    [SerializeField]
    public List<Collectible> collectibles;
}
