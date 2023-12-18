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
    [SerializeField]
    private Transform directionIndicator;
    private SpriteRenderer directionIndicatorSpriteRenderer;
    [SerializeField]
    private List<Sprite> directionIndicatorSprites = new List<Sprite>();

    public float distance;
    Vector2 direction;
    Quaternion rotation;
    [HideInInspector]
    public float angle;


    private void Awake()
    {
        if(directionIndicator != null)
            directionIndicatorSpriteRenderer = directionIndicator.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        distance = Vector2.Distance(player.transform.position, player.aimInputScreenPosition);
        direction = (player.aimInputScreenPosition - player.transform.position).normalized;
        angle = MathExtensions.GetAngle(transform.forward, direction);


        rotation = Quaternion.LookRotation(player.aimInputScreenPosition - player.transform.position, transform.TransformDirection(Vector3.up));

        if(GameManagerScript.instance.isMainMenu)
        {
            return;
        }
        if(!player.playerData.unlockedBow)
        {
            return;
        }

        UpdateReticlePosition();
        UpdateBowRotation();
        if (directionIndicator != null)
        {
            UpdateDirectionIndicator();
        }
    }

    void UpdateReticlePosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, distance, LayerMask.GetMask("Wall"));

        //Debug.Log($"{player.transform.position}/{player.aimInputScreenPosition}");

        if (hit.collider != null)
        {
            reticle.position = new Vector3(hit.point.x, hit.point.y, reticle.position.z);

            //Debug.DrawRay(player.transform.position, direction, Color.red, 5f);
        }
        else
        {
            reticle.position = new Vector3(player.aimInputScreenPosition.x, player.aimInputScreenPosition.y, reticle.position.z);

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

    void UpdateDirectionIndicator()
    {
        if(distance > 3)
        {
            directionIndicatorSpriteRenderer.sprite = directionIndicatorSprites[2];
        }
        else if (distance > 2f)
        {
            directionIndicatorSpriteRenderer.sprite = directionIndicatorSprites[1];
        }
        else if (distance > 1f)
        {
            directionIndicatorSpriteRenderer.sprite = directionIndicatorSprites[0];
        }

        if (distance > 1)
        {
            directionIndicatorSpriteRenderer.color = new Color(directionIndicatorSpriteRenderer.color.r, directionIndicatorSpriteRenderer.color.g, directionIndicatorSpriteRenderer.color.b, 1f);
        }
        else if (distance > 0)
        {
            directionIndicatorSpriteRenderer.color = new Color(directionIndicatorSpriteRenderer.color.r, directionIndicatorSpriteRenderer.color.g, directionIndicatorSpriteRenderer.color.b, 0f);
        }


        directionIndicator.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, directionIndicator.transform.position.z);
        directionIndicator.eulerAngles = new Vector3(0,0,angle);
    }
}
