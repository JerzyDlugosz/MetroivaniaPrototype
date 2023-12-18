using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class Custom2DCharacterController : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private PlayerData playerData;

    private Vector2 motion;

    public float movementSpeed;
    public float jumpSpeed;
    public float ladderSpeed;

    private Collider2D playerCollider;
    [SerializeField]
    private LayerMask WallLayer;

    private float jumpTimer;

    private bool stopMovement;

    public float moveSpeedModifier = 1f;
    public float jumpSpeedModifier = 1f;
    public float gravityModifier = 1f;

    public float additionalJumpPadModifier = 1f;

    public bool performedDoubleJump = false;

    public float dashTimer = 2f;
    public float dashCooldown = 0f;
    public int maxDashes = 3;
    public int currentDashes = 3;

    public float dashForce;
    public GameObject dashParticle;


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

    private void Update()
    {
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
        DashRecharge();

        playerData.playerGlobalPosition = new Vector2(transform.position.x, transform.position.y);
        playerData.playerMapPosition = new Vector2(((transform.position.x + 12) % 24) - 12, ((transform.position.y + 12) % 24) - 12);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(stopMovement)
        {
            return;
        }
        CheckIfPlayerIsGrounded();
        CheckIfPlayerIsOnLadder();

        StateModifiers();

        MotionForAnimations();
        CheckIfIdle();

        ApplyForce();
    }

    void StateModifiers()
    {
        // in water, in lava, in forcefield, is reloading, on jump pad, on ladder
        float waterMoveSpeedModifier = 1f;
        float waterJumpSpeedModifier = 1f;
        float waterGravityModfifier = 1f;

        float lavaMoveSpeedModifier = 1f;
        float lavaJumpSpeedModifier = 1f;
        float lavaGravityModfifier = 1f;

        float forcefieldMoveSpeedModifier = 1f;
        float forcefieldJumpSpeedModifier = 1f;
        float forcefieldGravityModfifier = 1f;

        float reloadingMoveSpeedModifier = 1f;
        float reloadingJumpSpeedModifier = 1f;
        float reloadingGravityModifier = 1f;

        float jumpPadMoveSpeedModifier = 1f;
        float jumpPadJumpSpeedModifier = 1f;
        float jumpPadGravityModifier = 1f;

        float ladderMoveSpeedModifier = 1f;
        float ladderJumpSpeedModifier = 1f;
        float ladderGravityModifier = 1f;

        float airStoneMoveSpeedModifier = 1f;
        float airStoneJumpSpeedModifier = 1f;
        float airStoneGravityModifier = 1f;

        if(player.inWater)
        {
            if (playerData.FireSpirit)
            {
                waterMoveSpeedModifier = 0.8f;
                waterJumpSpeedModifier = 0.8f;
                waterGravityModfifier = 0.2f;
            }
            else
            {
                waterMoveSpeedModifier = 0.5f;
                waterJumpSpeedModifier = 0.5f;
                waterGravityModfifier = 0.1f;
            }
        }
        if (player.inLava)
        {
            if(playerData.FireSpirit)
            {
                lavaMoveSpeedModifier = 0.66f;
                lavaJumpSpeedModifier = 0.66f;
                lavaGravityModfifier = 0.1f;
            }
            else
            {
                lavaMoveSpeedModifier = 0.33f;
                lavaJumpSpeedModifier = 0.33f;
                lavaGravityModfifier = 0.05f;
            }
        }
        if (player.inForceField)
        {
            forcefieldMoveSpeedModifier = player.inForceFieldModifier;
            forcefieldJumpSpeedModifier = player.inForceFieldModifier;
            forcefieldGravityModfifier = 1f;
        }
        if(playerData.isRecallButtonHeld)
        {
            if (!playerData.EarthSpirit)
            {
                reloadingMoveSpeedModifier = 0f;
                reloadingJumpSpeedModifier = 0f;
                reloadingGravityModifier = 1f;
            }
            else
            {
                reloadingMoveSpeedModifier = 0.5f;
                reloadingJumpSpeedModifier = 0.5f;
                reloadingGravityModifier = 1f;
            }
        }
        if (player.onJumpPad)
        {
            jumpPadMoveSpeedModifier = 1f;
            jumpPadJumpSpeedModifier = 3f * additionalJumpPadModifier;
            jumpPadGravityModifier = 1f;
        }
        if (player.onLadder)
        {
            ladderMoveSpeedModifier = 1f;
            ladderJumpSpeedModifier = 1f;
            ladderGravityModifier = 0f;
        }
        //if(playerData.AirSpirit)
        //{
        //    airStoneMoveSpeedModifier = 1.5f;
        //    airStoneJumpSpeedModifier = 1.5f;
        //    airStoneGravityModifier = 0.8f;
        //}
        if (playerData.isDownButtonHeld)
        {
            waterGravityModfifier = 1f;
            lavaGravityModfifier = 1f;
            airStoneGravityModifier = 1f;
        }


        moveSpeedModifier = waterMoveSpeedModifier * lavaMoveSpeedModifier * forcefieldMoveSpeedModifier * reloadingMoveSpeedModifier * jumpPadMoveSpeedModifier * ladderMoveSpeedModifier * airStoneMoveSpeedModifier;
        jumpSpeedModifier = waterJumpSpeedModifier * lavaJumpSpeedModifier * forcefieldJumpSpeedModifier * reloadingJumpSpeedModifier * jumpPadJumpSpeedModifier * ladderJumpSpeedModifier * airStoneJumpSpeedModifier;
        gravityModifier = waterGravityModfifier * lavaGravityModfifier * forcefieldGravityModfifier * reloadingGravityModifier * jumpPadGravityModifier * ladderGravityModifier * airStoneGravityModifier;

    }

    void CheckIfIdle()
    {
        if(motion.y == 0)
        {
            playerData.playerState = PlayerStates.Idle;
        }
    }

    void CheckIfPlayerIsOnLadder()
    {
        if(player.onLadder)
        {
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

    void MotionForAnimations()
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
    void CheckIfPlayerIsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 0.2f, WallLayer);

        if (hit.collider)
        {
            player.inAir = false;
            performedDoubleJump = false;
            player.isGrounded = true;
        }
        else
        {
            player.inAir = true;
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
    }

    void ApplyForce()
    {
        jumpSpeed = playerData.baseJumpSpeed * jumpSpeedModifier;
        movementSpeed = playerData.baseMovementSpeed * moveSpeedModifier;
        player.customRigidbody.gravityScale = playerData.baseGravityScale * gravityModifier;

        if(player.inAir)
            player.customRigidbody.drag = playerData.baseInAirDrag;
        else
            player.customRigidbody.drag = playerData.baseOnGroundDrag;

        //if (jumpTimer <= 0 && playerData.isJumping && player.isGrounded && jumpSpeedModifier > 0)
        //{
        //    jumpTimer = 0.2f;
        //    player.playerAnimation.OnPlayerJump();
        //    player.customRigidbody.AddForce(jumpSpeed * Vector2.up);
        //    GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(player.onJumpAudio);
        //}

        if (motion != Vector2.zero)
        {
            player.customRigidbody.AddForce(movementSpeed * Time.deltaTime * motion);
        }

        if (player.customRigidbody.velocity == Vector2.zero)
        {
            playerData.playerState = PlayerStates.Idle;
        }
    }

    public void Jump()
    {
        if (jumpTimer <= 0 && player.isGrounded && jumpSpeedModifier > 0)
        {
            jumpTimer = 0.2f;
            player.playerAnimation.OnPlayerJump();
            player.customRigidbody.AddForce(jumpSpeed * Vector2.up);
            GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(player.onJumpAudio);
        }
        else if (playerData.AirSpirit && !performedDoubleJump)
        {
            player.playerAnimation.OnPlayerJump();
            player.customRigidbody.AddForce(jumpSpeed * Vector2.up);
            GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(player.onJumpAudio);
            performedDoubleJump = true;
        }
    }

    public void DashRecharge()
    {
        if(currentDashes < maxDashes)
        {
            if (dashCooldown <= 0)
            {
                dashCooldown += dashTimer;
                currentDashes += 1;
                player.dashController.FillDashImage();
                return;
            }

            if (dashCooldown > 0)
            {
                dashCooldown -= Time.deltaTime;
            }
        }
    }

    public void LeftDash()
    {
        if(currentDashes > 0)
        {
            player.customRigidbody.AddForce(dashForce * Vector2.left);
            currentDashes -= 1;
            player.dashController.EmptyDashImage();
            Instantiate(dashParticle, transform.position, Quaternion.identity);
            if (playerData.AirSpirit)
                InvincibilityTimer(0.4f);
            else
                InvincibilityTimer(0.05f);
        }
    }

    public void RightDash()
    {
        if (currentDashes > 0)
        {
            player.customRigidbody.AddForce(dashForce * Vector2.right);
            currentDashes -= 1;
            player.dashController.EmptyDashImage();
            Instantiate(dashParticle, transform.position, Quaternion.identity);
            if(playerData.AirSpirit)
                InvincibilityTimer(0.4f);
            else
                InvincibilityTimer(0.05f);
        }
    }

    public void InvincibilityTimer(float invTime)
    {
        player.Invincibility(true);
        StartCoroutine(InvincibilityCouroutine(invTime));
    }

    IEnumerator InvincibilityCouroutine(float invTime)
    {
        yield return new WaitForSeconds(invTime);
        player.Invincibility(false);
    }

    public void ForceMovePlayer(Vector2 position)
    {
        player.customRigidbody.MovePosition(player.customRigidbody.position + position);
    }

    /// <summary>
    /// Transports player to a position while retaining his direction
    /// </summary>
    /// <param name="position"></param>
    public void ForceTransportPlayer(Vector2 position)
    {
        //player.customRigidbody.bodyType = RigidbodyType2D.Static;
        transform.position = player.customRigidbody.position + position;
        //player.customRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    /// Transports player to a position
    /// </summary>
    /// <param name="position"></param>
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
