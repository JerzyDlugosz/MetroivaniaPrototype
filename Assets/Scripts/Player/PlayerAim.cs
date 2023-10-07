using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlayerAim : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private Transform reticle;
    [SerializeField]
    private Transform weapon;

    public float distance;
    Vector2 direction;
    Quaternion rotation;
    [HideInInspector]
    public float angle;

    private void Update()
    {
        distance = Vector2.Distance(player.transform.position, player.aimInputScreenPosition);
        direction = (player.aimInputScreenPosition - player.transform.position).normalized;
        angle = MathExtensions.GetAngle(transform.forward, direction);


        rotation = Quaternion.LookRotation(player.aimInputScreenPosition - player.transform.position, transform.TransformDirection(Vector3.up));

        UpdateReticlePosition();
        UpdateBowRotation();
    }

    void UpdateReticlePosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, distance, LayerMask.GetMask("Wall"));

        //Debug.Log($"{player.transform.position}/{player.aimInputScreenPosition}");

        if (hit.collider != null)
        {
            reticle.position = hit.point;

            //Debug.DrawRay(player.transform.position, direction, Color.red, 5f);
        }
        else
        {
            reticle.position = player.aimInputScreenPosition;

            //Debug.DrawRay(player.transform.position, direction, Color.white, 5f);
        }
    }

    void UpdateBowRotation()
    {
        weapon.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

        if (direction.x > 0)
        {
            if(player.playerSpriteRenderer.flipX)
            {
                player.playerSpriteRenderer.flipX = false;
                weapon.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        if (direction.x < 0)
        {
            if (!player.playerSpriteRenderer.flipX)
            {
                player.playerSpriteRenderer.flipX = true;
                weapon.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }

    public float getAngle(Vector2 me, Vector2 target)
    {
        return Mathf.Atan2(target.y - me.y, target.x - me.x) * (180 / Mathf.PI);
    }
}
