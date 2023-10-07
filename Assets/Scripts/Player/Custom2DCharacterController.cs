using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Custom2DCharacterController : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private PlayerData playerData;

    private Vector3 velocity;

    private Vector2 motion;
    private Vector2 JumpMotion;
    private Vector3 animationMotion;
    private Vector3 gravity;

    public float movementSpeed;
    public float jumpSpeed;
    public float ladderSpeed;

    private Collider2D playerCollider;
    [SerializeField]
    private LayerMask WallLayer;

    private float jumpTimer;

    private bool stopMovement;
    
    public void StopMovement(bool state)
    {
        stopMovement = state;

        if(state)
        {
            player.customRigidbody.bodyType = RigidbodyType2D.Static;
            player.customRigidbody.isKinematic = true;
            player.customRigidbody.velocity = Vector3.zero;
            //player.customRigidbody.gravityScale = 0;
        }
        else
        {
            player.customRigidbody.bodyType = RigidbodyType2D.Static;
            player.customRigidbody.isKinematic = false;
            //player.customRigidbody.gravityScale = playerData.baseGravityScale;
        }
    }

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

        if(stopMovement)
        {
            return;
        }

        Movement();
        Jump();
        ApplyForce();
        Climbing();
        Swimming();

        ResetGravity();

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }

        playerData.playerGlobalPosition = new Vector2(transform.position.x, transform.position.y);
        playerData.playerMapPosition = new Vector2(((transform.position.x + 12) % 24) - 12, ((transform.position.y + 12) % 24) - 12);
    }

    void CheckPosition()
    {
        if(motion.y == 0)
        {
            playerData.playerState = PlayerStates.Idle;
        }
    }

    void Climbing()
    {
        if(player.onLadder)
        {
            player.customRigidbody.gravityScale = 0f;
            if (playerData.isUpButtonHeld)
            {
                player.customRigidbody.velocity = new Vector2(player.customRigidbody.velocity.x, ladderSpeed);
            }
            if(playerData.isDownButtonHeld)
            {
                player.customRigidbody.velocity = new Vector2(player.customRigidbody.velocity.x, -ladderSpeed);
            }
        }
    }

    void Swimming()
    {
        if (player.inWater)
        {
            Debug.Log("In Water");
            playerData.playerState = PlayerStates.InWater;
            player.customRigidbody.gravityScale = playerData.baseGravityScale / 10f;
            jumpSpeed = playerData.baseJumpSpeed / 2;
            movementSpeed = playerData.baseMovementSpeed / 2;

            if (playerData.isDownButtonHeld)
            {
                player.customRigidbody.gravityScale = playerData.baseGravityScale;
            }
        }

        if(player.inLava)
        {
            Debug.Log("In Lava");
            playerData.playerState = PlayerStates.InLava;
            player.customRigidbody.gravityScale = playerData.baseGravityScale / 20f;
            jumpSpeed = playerData.baseJumpSpeed / 3;
            movementSpeed = playerData.baseMovementSpeed / 3;

            if (playerData.isDownButtonHeld)
            {
                player.customRigidbody.gravityScale = playerData.baseGravityScale;
            }
        }
    }

    void ResetGravity()
    {
        if (!player.onLadder && !player.inWater && !player.inLava && !player.onJumpPad)
        {
            player.customRigidbody.gravityScale = player.playerData.baseGravityScale;
            jumpSpeed = playerData.baseJumpSpeed;
            movementSpeed = playerData.baseMovementSpeed;
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
            player.isGrounded = true;
        }
        else
        {
            playerData.playerState = PlayerStates.InAir;
            player.isGrounded = false;
        }

        if (player.inWater && playerData.WaterSpirit)
        {
            player.isGrounded = true;
        }

        if (player.inLava && playerData.FireSpirit)
        {
            player.isGrounded = true;
        }

        if(player.onJumpPad)
        {
            player.characterController.jumpSpeed = playerData.baseJumpSpeed * 2;
        }
    }

    void ApplyForce()
    {
        if (jumpTimer <= 0 && playerData.isJumping && player.isGrounded)
        {
            jumpTimer = 0.2f;
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
        //player.customRigidbody.bodyType = RigidbodyType2D.Static;
        transform.position = player.customRigidbody.position + position;
        //player.customRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void ForceTransportPlayerToPosition(Vector2 position)
    {
        //player.customRigidbody.bodyType = RigidbodyType2D.Static;
        transform.position = position;
        //player.customRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void ForceApplyForce(Vector2 motion)
    {
        player.customRigidbody.AddForce(Time.deltaTime * motion);
    }
}

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}
