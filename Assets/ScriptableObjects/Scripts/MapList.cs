using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MapArray")]
public class MapList : ScriptableObject
{
    [SerializeField]
    public List<Map> maps = new List<Map>();

}
