using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private PlayerData playerData;

    private bool groundedPlayer;
    private Vector3 velocity;

    private Vector3 motion;
    private Vector3 animationMotion;
    private Vector3 gravity;

    public float speed = 2f;
    public float runningSpeedMultiplier = 2f;

    [HideInInspector]
    public bool isRunning = false;
    [HideInInspector]
    public bool isRotationEnabled = true;


    [SerializeField]
    private float speedIncreaseModifier = 2;
    [SerializeField]
    private float speedDecreaseModifier = 2;
    [SerializeField]
    private float runningSpeedIncreaseModifier = 2;
    [SerializeField]
    private float runningSpeedDecreaseModifier = 2;

    private float currentRunningSpeed = 1f;
    private float currentSpeed;

    void Start()
    {
        playerData = player.playerData;

        playerData.playerState = PlayerStates.Idle;
        gravity = new Vector3(0, -1, 0);
    }

    void Update()
    {
        CheckPlayerYVelocity();
        Move();
        Gravity();
    }

    void CheckPlayerYVelocity()
    {
        if (groundedPlayer && velocity.y < 0)
        {
            velocity.y = 0f;
        }
    }

    public void Move()
    {
        if (playerData.isRunning)
        {
            playerData.playerState = PlayerStates.Running;
        }
        else
        {
            playerData.playerState = PlayerStates.Walking;
        }

        motion = player.movementInput.x * Vector3.right;
        //motion = player.movementInput.x * transform.right + player.movementInput.y * transform.forward;
        //animationMotion = player.movementInput.x * Vector3.right + player.movementInput.y * Vector3.forward;


        if (motion != Vector3.zero)
        {
            motion.Normalize();

            if (currentSpeed < speed)
                currentSpeed += Time.deltaTime * speedIncreaseModifier;
            else
                currentSpeed = speed;

            if (playerData.isRunning)
            {
                if (currentRunningSpeed < runningSpeedMultiplier)
                    currentRunningSpeed += Time.deltaTime * runningSpeedIncreaseModifier;
                else
                    currentRunningSpeed = runningSpeedMultiplier;

                motion *= currentRunningSpeed;
            }
            else
            {
                currentRunningSpeed = 1f;
            }


        }
        else
        {
            playerData.playerState = PlayerStates.Idle;

            if (currentSpeed > 0f) currentSpeed = 0f;
            if (currentRunningSpeed > 1f) currentRunningSpeed = 1f;

            //if(currentSpeed > 0f)
            //    currentSpeed -= Time.deltaTime * speedDecreaseModifier;
            //else
            //    currentSpeed = 0f;
            //if(currentRunningSpeed > 0f)
            //    currentRunningSpeed -= Time.deltaTime * runningSpeedDecreaseModifier;
            //else
            //    currentRunningSpeed = 0f;
        }

        playerData.motion = motion;
        playerData.animationMotion = animationMotion;
    }

    void Gravity()
    {

    }
}