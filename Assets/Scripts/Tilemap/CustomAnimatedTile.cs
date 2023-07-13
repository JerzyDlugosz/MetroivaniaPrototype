using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimatedTile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float scale;
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    private void Update()
    {
        foreach (var item in sprites)
        {
            
        }
    }
}
