using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TransportBeam : MonoBehaviour
{
    [SerializeField]
    private TilemapRenderer tilemapRenderer;
    [SerializeField]
    private Collider2D beamCollider;

    public void UnlockTransportBeam()
    {
        tilemapRenderer.enabled = true;
        beamCollider.enabled = true;
    }
}
