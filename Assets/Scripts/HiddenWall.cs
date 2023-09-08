using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenWall : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    private void Start()
    {
        tilemapRenderer= GetComponent<TilemapRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            tilemapRenderer.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            tilemapRenderer.enabled = true;
    }
}
