using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDebug : MonoBehaviour
{
    [SerializeField]
    private Player player;


    private void Update()
    {

        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, player.aimInputScreenPosition, 10f, LayerMask.GetMask("Wall"));

        Debug.Log($"{hit.collider}/{hit.distance}");
        Debug.DrawRay(player.transform.position, player.aimInputScreenPosition, Color.white, 5f);
        Debug.DrawRay(player.transform.position, hit.point, Color.red, 5f);
    }

}
