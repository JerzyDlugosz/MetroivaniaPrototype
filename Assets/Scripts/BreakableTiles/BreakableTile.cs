using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

[Serializable]
public class OnTileHitEvent : UnityEvent { }

[Serializable]
public class OnTileDestroyEvent : UnityEvent { }

public class BreakableTile : MonoBehaviour
{
    public BreakableTileData tileData;
    public OnTileHitEvent onHitEvent;
    public OnTileDestroyEvent onDestroyEvent;

    /// <summary>
    /// Describes the required strength of a projectile to break this tile
    /// </summary>
    [SerializeField]
    private int tileStrength;
    [SerializeField]
    private int tileStickiness;

    [SerializeField]
    private Tilemap tilemap;

    [SerializeField]
    private List<TileBase> onDestroyTiles;
    [SerializeField]
    private List<Vector3Int> tilePos = new List<Vector3Int>();

    [SerializeField]
    private float timeBetweenAnimFrames;

    [SerializeField]
    private bool isObjectComposite = false;

    private void Start()
    {
        if(tileData == null)
        {
            if(TryGetComponent(out BreakableTileData comp))
            {
                tileData = comp;
            }
        }

        //Set this in inspector
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if(tilemap.GetTile(pos) != null)
            {
                tilePos.Add(pos);
            }
        }

        onDestroyEvent.AddListener(RemoveCollisions);
    }

    private void RemoveCollisions()
    {
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            Destroy(collider);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.collider.CompareTag("Projectile"))
        {
            if(collision.collider.TryGetComponent(out PlayerProjectile playerProjectile))
            {
                if (CheckIfProjectileCanDestroyThisTile(playerProjectile.projectileStrenght))
                {
                    var pos = tilemap.WorldToCell(collision.GetContact(0).point);

                    Debug.Log("can!");

                    if (tilemap.GetTile(pos) != null)
                    {
                        if(isObjectComposite)
                        {
                            foreach (var item in tilePos)
                            {
                                StartCoroutine(WallDestroyedAnim(item));
                            }
                            playerProjectile.wallCollisionEvent.Invoke();
                        }
                        else
                        {
                            
                            StartCoroutine(WallDestroyedAnim(pos));
                            playerProjectile.wallCollisionEvent.Invoke();
                        }
                    }
                }
                else if (CheckIfProjectileCanStickToThisTile(playerProjectile.projectileStickiness))
                {
                    playerProjectile.wallCollisionEvent.Invoke();
                }
            }
        }
    }

    private bool CheckIfProjectileCanDestroyThisTile(int projectileStrenght)
    {
        if(projectileStrenght >= tileStrength)
        {
            return true;
        }
        return false;
    }

    private bool CheckIfProjectileCanStickToThisTile(int projectileStickiness)
    {
        if (projectileStickiness >= tileStickiness)
        {
            return true;
        }
        return false;
    }

    IEnumerator WallDestroyedAnim(Vector3Int pos)
    {
        for (int i = 0; i < onDestroyTiles.Count; i++)
        {
            tilemap.SetTile(pos, onDestroyTiles[i]);
            yield return new WaitForSeconds(timeBetweenAnimFrames);
        }
    }
}
