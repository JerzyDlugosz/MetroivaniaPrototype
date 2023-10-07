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
public class OnArrowtipUsedEvent : UnityEvent { }
/// <summary>
/// Old event, may still be usable
/// </summary>
[Serializable]
public class OnArrowRenewEvent : UnityEvent { }
[Serializable]
public class OnArrowsRenewEvent : UnityEvent { }
[Serializable]
public class OnArrowtipPickupEvent : UnityEvent { }
[Serializable]
public class OnArrowChangeLeftEvent : UnityEvent { }
[Serializable]
public class OnArrowChangeRightEvent : UnityEvent { }
[Serializable]
public class OnDamageTakenEvent : UnityEvent<float> { }
[Serializable]
public class OnMaxHealthUpdateEvent : UnityEvent { }
[Serializable]
public class OnHealthPickupEvent : UnityEvent<float> { }
public class Player : BaseEntity
{
    public PlayerData playerData;
    public PlayerAnimation playerAnimation;
    public Custom2DCharacterController characterController;
    public PlayerAim playerAim;
    public PlayerShooting playerShooting;
    public PlayerWeaponSwap playerWeaponSwap;
    public ProgressTracker progressTracker;

    public BoxCollider2D customCollider;

    public GameObject cameraHolder;
    public GameObject mainCamera;
    public PlayerInputActions playerInputActions;
    public PlayerInput playerInput;  


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
    public float additionalDamageTakenDelay;

    [HideInInspector]
    public OnArrowCapacityIncreaseEvent arrowCapacityIncreaseEvent;
    [HideInInspector]
    public OnArrowTypeCollectedEvent arrowTypeCollectedEvent;
    [HideInInspector]
    public OnArrowUsedEvent arrowUsedEvent;
    [HideInInspector]
    public OnArrowtipUsedEvent arrowtipUsedEvent;
    [HideInInspector]
    public OnArrowRenewEvent arrowRenewEvent;
    [HideInInspector]
    public OnArrowRenewEvent arrowsRenewEvent;
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
    [HideInInspector]
    public OnArrowtipPickupEvent arrowtipPickupEvent;

    /// <summary>
    /// TemporaryFix, events dont work cause its not in order
    /// </summary>
    public RemainingHearthsScript remainingHearthsScript;

    /// <summary>
    /// TemporaryFix, events dont work cause its not in order
    /// </summary>
    public RemainingArrowScript remainingArrowScript;
    public override void Start()
    {
        base.Start();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.PauseMenu.Disable();
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

        playerInputActions.Player.Pause.canceled += Pause_canceled;
        playerInputActions.PauseMenu.UnPause.canceled += UnPause_canceled;

        playerInputActions.PauseMenu.ChangeMenuLeft.performed += ChangeMenuLeft_performed;
        playerInputActions.PauseMenu.ChangeMenuRight.performed += ChangeMenuRight_performed;


        arrowCapacityIncreaseEvent.AddListener(OnArrowCapacityIncrease);
        arrowTypeCollectedEvent.AddListener(OnArrowTypeCollected);
        arrowUsedEvent.AddListener(OnArrowUsed);
        arrowtipUsedEvent.AddListener(OnArrowtipUsed);
        arrowRenewEvent.AddListener(OnArrowRenew);
        arrowsRenewEvent.AddListener(OnArrowsRenew);
        arrowChangeLeftEvent.AddListener(() => OnArrowChange(-1));
        arrowChangeRightEvent.AddListener(() => OnArrowChange(1));
        damageTakenEvent.AddListener(TakeDamage);
        maxHealthUpdateEvent.AddListener(MaxHealthPickup);
        healthPickupEvent.AddListener(HealthPickup);
        arrowtipPickupEvent.AddListener(OnArrowtipPickup);

        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }
        ReadInput();
    }

    private void OnStop(bool state)
    {
        if (state)
        {
            customRigidbody.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            customRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void OnBowUnlocked(bool value)
    {
        bow.SetActive(value);
        playerData.unlockedBow = value;
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

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        
    }

    private void Pause_canceled(InputAction.CallbackContext obj)
    {
        Debug.Log(playerInput.currentActionMap.name);
        playerInput.SwitchCurrentActionMap("PauseMenu");
        GameManagerScript.instance.entitiesManager.EntitiesPauseState(true);
        playerInputActions.Player.Disable();
        playerInputActions.PauseMenu.Enable();
        GameManagerScript.instance.pauseMenu.gameObject.SetActive(true);
        GameManagerScript.instance.pauseMenu.OnPauseScreenEnable();
    }

    private void UnPause_performed(InputAction.CallbackContext obj)
    {
        
    }

    private void UnPause_canceled(InputAction.CallbackContext obj)
    {
        GameManagerScript.instance.pauseMenu.gameObject.SetActive(false);
        Debug.Log(playerInput.currentActionMap.name);
        playerInput.SwitchCurrentActionMap("Player");
        GameManagerScript.instance.entitiesManager.EntitiesPauseState(false);
        playerInputActions.Player.Enable();
        playerInputActions.PauseMenu.Disable();
    }

    private void ChangeMenuLeft_performed(InputAction.CallbackContext obj)
    {
        GameManagerScript.instance.pauseMenu.SwitchMenu(-1);
    }

    private void ChangeMenuRight_performed(InputAction.CallbackContext obj)
    {
        GameManagerScript.instance.pauseMenu.SwitchMenu(1);
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

    void OnArrowtipUsed()
    {
        playerData.currentArrowtipsCount--;
    }

    void OnArrowRenew()
    {
        playerData.currentArrowCount++;
        if (playerData.currentArrowCount > playerData.maxArrowCount)
        {
            playerData.currentArrowCount = playerData.maxArrowCount;
        }
    }
    public void OnArrowsRenew()
    {
        playerData.currentArrowCount = playerData.maxArrowCount;
    }

    public void OnArrowtipPickup()
    {
        playerData.currentArrowtipsCount += 2;
        if (playerData.currentArrowtipsCount > playerData.maxArrowtipsCount)
        {
            playerData.currentArrowtipsCount = playerData.maxArrowtipsCount;
        }
    }

    public void OnArrowtipsRenew()
    {
        playerData.currentArrowtipsCount = playerData.maxArrowtipsCount;
    }

    void OnArrowCapacityIncrease()
    {
        playerData.currentArrowCount++;
        playerData.maxArrowCount++;

        playerData.currentArrowtipsCount += 2;
        playerData.maxArrowtipsCount += 2;
    }
    void OnArrowTypeCollected()
    {
        if(playerWeaponSwap.unlockedWeapons < playerWeaponSwap.availableWeapons.Length)
        {
            playerWeaponSwap.unlockedWeapons++;
            remainingArrowScript.EnableArrowTypesUI(playerWeaponSwap.unlockedWeapons);

        }
        else
        {
            Debug.LogWarning("Player has collected more arrow type upgrades than the ammount of avaiable types");
        }
    }

    public void RefreshUI()
    {
        remainingArrowScript.OnGameLoad(playerData.maxArrowCount, playerWeaponSwap.unlockedWeapons);
        remainingHearthsScript.UpdateCurrentHealthOnImages((int)playerData.health);
    }

    void OnArrowChange(int changeDirection)
    {
        playerWeaponSwap.OnWeaponSwitch(changeDirection);
        remainingArrowScript.ChangeImagePrefab();
    }

    public override void TakeDamage(float damage)
    {
        if (damageable)
        {
            playerData.health -= damage;

            if(playerData.health <= 0)
            {
                GameStateManager.instance.audioManager.PlaySoundEffect(onDeathAudioClip);
                Debug.Log("You are dead!");
            }

            remainingHearthsScript.UpdateCurrentHealthOnImages((int)playerData.health);
            StartCoroutine(DamageTimer());
            GameStateManager.instance.audioManager.PlaySoundEffect(onHitAudioClip);
        }
    }

    public void MaxHealthPickup()
    {
        playerData.maxHealth += 4;
        HealthPickup(4);
    }

    public void HealthPickup(float healthAmmount)
    {
        float tempHealth = playerData.health += healthAmmount;
        if(tempHealth > playerData.maxHealth)
        {
            playerData.health = playerData.maxHealth;
        }
        else
        {
            playerData.health = tempHealth;
        }
        remainingHearthsScript.UpdateCurrentHealthOnImages((int)playerData.health);
    }

    IEnumerator DamageTimer()
    {
        damageable = false;
        playerAnimation.playerSpriteRenderer.material.SetFloat(flashID, 1);
        remainingHearthsScript.FlashHearths(flashID, true);
        yield return new WaitForSeconds(0.1f);
        playerAnimation.playerSpriteRenderer.material.SetFloat(flashID, 0);
        remainingHearthsScript.FlashHearths(flashID, false);
        yield return new WaitForSeconds(additionalDamageTakenDelay);
        damageable = true;
    }

    public void PlayAttackSoundEffect()
    {
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }
}
