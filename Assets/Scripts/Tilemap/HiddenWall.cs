using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenWall : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    public bool Maskable;

    private void Start()
    {
        tilemapRenderer= GetComponent<TilemapRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Maskable)
        {
            if (collision.CompareTag("Player"))
                collision.GetComponentInParent<Player>().HiddenWallMask.SetActive(true);
        }
        else
        {
            if (collision.CompareTag("Player"))
                tilemapRenderer.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Maskable)
        {
            if (collision.CompareTag("Player"))
                collision.GetComponentInParent<Player>().HiddenWallMask.SetActive(false);
        }
        else
        {
            if (collision.CompareTag("Player"))
                tilemapRenderer.enabled = true;
        }
    }
}
