using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

[Serializable]
public class OnArrowCapacityIncreaseEvent : UnityEvent { }
[Serializable]
public class OnArrowTypeCollectedEvent : UnityEvent { }
[Serializable]
public class OnArrowUsedEvent : UnityEvent { }
[Serializable]
public class OnArrowRenewEvent : UnityEvent { }
[Serializable]
public class OnArrowChangeLeftEvent : UnityEvent { }
[Serializable]
public class OnArrowChangeRightEvent : UnityEvent { }
[Serializable]
public class OnDamageTakenEvent : UnityEvent<float> { }
[Serializable]
public class OnMaxHealthUpdateEvent : UnityEvent { }
[Serializable]
public class OnHealthPickupEvent : UnityEvent { }
public class Player : BaseEntity
{
    public PlayerData playerData;
    public PlayerAnimation playerAnimation;
    public Custom2DCharacterController characterController;
    public PlayerAim playerAim;
    public PlayerShooting playerShooting;
    public CollectedItems collectedItems;
    public PlayerWeaponSwap playerWeaponSwap;
    public ProgressTracker progressTracker;

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

    [SerializeField]
    private GameObject bow;

    private bool damageable = true;

    [HideInInspector]
    public OnArrowCapacityIncreaseEvent arrowCapacityIncreaseEvent;
    [HideInInspector]
    public OnArrowTypeCollectedEvent arrowTypeCollectedEvent;
    [HideInInspector]
    public OnArrowUsedEvent arrowUsedEvent;
    [HideInInspector]
    public OnArrowRenewEvent arrowRenewEvent;
    [HideInInspector]
    public OnArrowChangeLeftEvent arrowChangeLeftEvent;
    [HideInInspector]
    public OnArrowChangeRightEvent arrowChangeRightEvent;
    [HideInInspector]
    public OnDamageTakenEvent damageTakenEvent;
    [HideInInspector]
    public OnMaxHealthUpdateEvent maxHealthUpdateEvent;
    [HideInInspector]
    public OnHealthPickupEvent healthPickupEvent;


    /// <summary>
    /// TemporaryFix, events dont work cause its not in order
    /// </summary>
    public RemainingHearthsScript remainingHearthsScript;
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

        playerInputActions.Player.DownMotion.performed += DownArrow_performed;
        playerInputActions.Player.DownMotion.canceled += DownArrow_canceled;

        playerInputActions.Player.UpMotion.performed += UpArrow_performed;
        playerInputActions.Player.UpMotion.canceled += UpArrow_canceled;

        playerInputActions.Player.Recall.performed += Recall_performed;
        playerInputActions.Player.Recall.canceled += Recall_canceled;

        arrowCapacityIncreaseEvent.AddListener(OnArrowCapacityIncrease);
        arrowTypeCollectedEvent.AddListener(OnArrowTypeCollected);
        arrowUsedEvent.AddListener(OnArrowUsed);
        arrowRenewEvent.AddListener(OnArrowRenew);
        arrowChangeLeftEvent.AddListener(() => playerWeaponSwap.OnWeaponSwitch(-1));
        arrowChangeRightEvent.AddListener(() => playerWeaponSwap.OnWeaponSwitch(1));
        damageTakenEvent.AddListener(TakeDamage);
        maxHealthUpdateEvent.AddListener(MaxHealthPickup);
        healthPickupEvent.AddListener(HealthPickup);
    }

    private void Update()
    {
        ReadInput();
    }

    public void OnBowUnlocked(bool value)
    {
        bow.SetActive(value);
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
        arrowChangeLeftEvent.Invoke();
    }

    private void WeaponSwapRight_performed(InputAction.CallbackContext obj)
    {
        arrowChangeRightEvent.Invoke();
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

    private void UpArrow_performed(InputAction.CallbackContext obj)
    {
        playerData.isUpButtonHeld = true;
    }

    private void UpArrow_canceled(InputAction.CallbackContext obj)
    {
        playerData.isUpButtonHeld = false;
    }

    private void Recall_performed(InputAction.CallbackContext obj)
    {
        playerData.isRecallButtonHeld = true;
    }

    private void Recall_canceled(InputAction.CallbackContext obj)
    {
        playerData.isRecallButtonHeld = false;
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

    void OnArrowUsed()
    {
        playerData.currentArrowCount--;
    }
    void OnArrowRenew()
    {
        playerData.currentArrowCount++;
    }
    void OnArrowCapacityIncrease()
    {
        playerData.currentArrowCount++;
        playerData.maxArrowCount++;
    }
    void OnArrowTypeCollected()
    {
        if(playerWeaponSwap.unlockedWeapons < playerWeaponSwap.availableWeapons.Length)
        {
            playerWeaponSwap.unlockedWeapons++;
        }
        else
        {
            Debug.LogWarning("Player has collected more arrow type upgrades than the ammount of avaiable types");
        }
    }

    public void TakeDamage(float damage)
    {
        if (damageable)
        {
            playerData.health -= (int)damage;

            if(playerData.health <= 0)
            {
                Debug.Log("You are dead!");
            }

            remainingHearthsScript.UpdateCurrentHealthOnImages(playerData.health);
            StartCoroutine(DamageTimer());
        }
    }

    public void MaxHealthPickup()
    {
        playerData.maxHealth += 4;
    }

    public void HealthPickup()
    {
        int tempHealth = playerData.health += 4;
        if(tempHealth > playerData.maxHealth)
        {
            playerData.health = playerData.maxHealth;
        }
        else
        {
            playerData.health = tempHealth;
        }
        remainingHearthsScript.UpdateCurrentHealthOnImages(playerData.health);
    }

    IEnumerator DamageTimer()
    {
        damageable = false;
        playerAnimation.playerSpriteRenderer.material.SetFloat(flashID, 1);
        remainingHearthsScript.FlashHearths(flashID, true);
        yield return new WaitForSeconds(0.1f);
        playerAnimation.playerSpriteRenderer.material.SetFloat(flashID, 0);
        remainingHearthsScript.FlashHearths(flashID, false);
        damageable = true;
    }
}
