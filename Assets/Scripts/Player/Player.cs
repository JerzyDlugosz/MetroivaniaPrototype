using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public PlayerAnimation playerAnimation;
    public Custom2DCharacterController characterController;
    public PlayerAim playerAim;

    public BoxCollider2D customCollider;

    public GameObject cameraHolder;
    public GameObject mainCamera;
    public PlayerInputActions playerInputActions;


    public Vector2 rotationInput;
    public Vector2 movementInput;

    public Vector2 aimInput;
    public Vector3 aimInputScreenPosition;

    [HideInInspector]
    public string currentInputControl;

    public Rigidbody2D customRigidbody;

    public SpriteRenderer playerSpriteRenderer;

    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Sprint.performed += Sprint_performed;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;

        playerInputActions.Player.Shoot.performed += Shooting_performed;
        playerInputActions.Player.Shoot.canceled += Shooting_canceled;

        playerInputActions.Player.WeaponSwapLeft.performed += WeaponSwapLeft_performed;
        playerInputActions.Player.WeaponSwapRight.performed += WeaponSwapRight_performed;

        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Jump.canceled += Jump_canceled;

        playerInputActions.Player.GetDownFromPlatform.performed += DownArrow_performed;
        playerInputActions.Player.GetDownFromPlatform.canceled += DownArrow_canceled;

        //playerData.currentWeapon = playerData.avaiableWeapons[0];

    }

    private void Update()
    {
        ReadInput();
    }

    private void Sprint_canceled(InputAction.CallbackContext obj)
    {
        playerData.isRunning = false;
    }

    private void Sprint_performed(InputAction.CallbackContext obj)
    {
        playerData.isRunning = true;
    }

    private void Shooting_canceled(InputAction.CallbackContext obj)
    {
        playerData.isShooting = false;
    }

    private void Shooting_performed(InputAction.CallbackContext obj)
    {
        playerData.isShooting = true;
    }

    private void WeaponSwapLeft_performed(InputAction.CallbackContext obj)
    {
        //playerWeaponSwitch.WeaponSwapLeft();
    }

    private void WeaponSwapRight_performed(InputAction.CallbackContext obj)
    {
        //playerWeaponSwitch.WeaponSwapRight();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        playerData.isJumping = true;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        playerData.isJumping = false;
    }

    private void DownArrow_performed(InputAction.CallbackContext obj)
    {
        playerData.isDownButtonHeld = true;
    }

    private void DownArrow_canceled(InputAction.CallbackContext obj)
    {
        playerData.isDownButtonHeld = false;
    }

    void ReadInput()
    {
        rotationInput = playerInputActions.Player.Rotation.ReadValue<Vector2>();
        movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
        aimInput = playerInputActions.Player.AimPosition.ReadValue<Vector2>();

        if (playerInputActions.Player.Rotation.activeControl != null)
        {
            currentInputControl = playerInputActions.Player.Rotation.activeControl.device.name;
        }
        aimInputScreenPosition = Camera.main.ScreenToWorldPoint(aimInput);
        aimInputScreenPosition.z = Camera.main.nearClipPlane;
    }
}
