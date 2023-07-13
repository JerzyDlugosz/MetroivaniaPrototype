using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Custom2DCharacterController : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private PlayerData playerData;

    private bool groundedPlayer;
    private Vector3 velocity;

    private Vector2 motion;
    private Vector2 JumpMotion;
    private Vector3 animationMotion;
    private Vector3 gravity;

    public float movementSpeed;
    public float jumpSpeed;

    private Collider2D playerCollider;
    [SerializeField]
    private LayerMask WallLayer;

    private float jumpTimer;
    

    void Start()
    {
        playerCollider = player.customCollider;
        playerData = player.playerData;

        playerData.playerState = PlayerStates.Idle;

    }

    // Update is called once per frame
    void Update()
    {
       // CheckPosition();

        Movement();
        Jump();
        ApplyForce();

        if(jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    void CheckPosition()
    {
        if(motion.y == 0)
        {
            playerData.playerState = PlayerStates.Idle;
        }
    }

    void Movement()
    {
        if (playerData.isRunning)
        {
            playerData.playerState = PlayerStates.Running;
        }
        else
        {
            playerData.playerState = PlayerStates.Walking;
        }

        motion = player.movementInput.x * Vector2.right;
        playerData.motion = motion;
        playerData.animationMotion = motion;
    }

    void Jump()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 0.2f, WallLayer);

        if(hit.collider)
        {
            Debug.Log("On ground");
            groundedPlayer = true;
        }
        else
        {
            Debug.Log("In Air");
            playerData.playerState = PlayerStates.InAir;
            groundedPlayer = false;
        }   
    }

    void ApplyForce()
    {
        if (jumpTimer <= 0 && playerData.isJumping && groundedPlayer)
        {
            jumpTimer = 1;
            player.playerAnimation.OnPlayerJump();
            player.customRigidbody.AddForce(jumpSpeed * Vector2.up);
        }

        if (motion != Vector2.zero)
        {
            player.customRigidbody.AddForce(movementSpeed * Time.deltaTime * motion);
        }

        if (player.customRigidbody.velocity == Vector2.zero)
        {
            playerData.playerState = PlayerStates.Idle;
        }
    }

    public void ForceMovePlayer(Vector2 position)
    {
        player.customRigidbody.MovePosition(player.customRigidbody.position + position);
    }

    public void ForceTransportPlayer(Vector2 position)
    {
        player.customRigidbody.bodyType = RigidbodyType2D.Static;
        transform.position = position;
        player.customRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
}

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}
