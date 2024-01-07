using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[Serializable]
public class OnArrowCapacityIncreaseEvent : UnityEvent { }
[Serializable]
public class OnArrowTypeCollectedEvent : UnityEvent { }
[Serializable]
public class OnArrowDamageIncreaseEvent : UnityEvent { }
[Serializable]
public class OnArrowReloadSpeedIncreaseEvent : UnityEvent { }
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
    protected static readonly int flashHealingID = Shader.PropertyToID("_FlashHealingStrength");
    protected static readonly int deathID = Shader.PropertyToID("_DeathStrength");

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

    public GameObject HiddenWallMask;

    [SerializeField]
    private GameObject bow;
    [SerializeField]
    private List<GameObject> reticleAndDirectionIndicators;
    [SerializeField]
    private List<GameObject> reticleAndDirectionIndicatorsChildren;

    private bool damageable = true;
    private bool healable = true;
    private bool isInvincible = false;
    private bool mapMovementAvailable = false;
    public float additionalDamageTakenDelay;

    #region AudioClips
    public AudioClip onHealthRestoreAudio;
    public AudioClip onJumpAudio;
    public AudioClip onBeginReloadAudio;
    public AudioClip onLoopReloadAudio;
    public AudioClip onEndReloadAudio;
    #endregion

    public bool reachedSecret = false;

    [HideInInspector]
    public OnArrowDamageIncreaseEvent arrowDamageIncreaseEvent;
    [HideInInspector]
    public OnArrowReloadSpeedIncreaseEvent arrowReloadSpeedIncreaseEvent;
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

    public DashController dashController;

    public GameObject deathScreen;

    private Vector2 previousAimPosition = Vector2.zero;

    private void Awake()
    {
        if(playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
    }

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        ChangeInput(InputMode.Game);
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


        //MapControls
        playerInputActions.PauseMenu.MoveMapUp.performed += MoveMapDown;

        playerInputActions.PauseMenu.MoveMapDown.performed += MoveMapUp;

        playerInputActions.PauseMenu.MoveMapLeft.performed += MoveMapLeft;

        playerInputActions.PauseMenu.MoveMapRight.performed += MoveMapRight;

        playerInputActions.PauseMenu.EnableMapMovement.performed += EnableMapMovement;
        playerInputActions.PauseMenu.EnableMapMovement.canceled += DisableMapMovement;
        //End




        playerInputActions.Player.Sprint.performed += OnAnyAction;
        playerInputActions.Player.Sprint.canceled += OnAnyAction;

        playerInputActions.Player.Shoot.performed += OnAnyAction;
        playerInputActions.Player.Shoot.canceled += OnAnyAction;

        playerInputActions.Player.WeaponSwapLeft.performed += OnAnyAction;
        playerInputActions.Player.WeaponSwapRight.performed += OnAnyAction;

        playerInputActions.Player.Jump.performed += OnAnyAction;
        playerInputActions.Player.Jump.canceled += OnAnyAction;

        playerInputActions.Player.DownMotion.performed += OnAnyAction;
        playerInputActions.Player.DownMotion.canceled += OnAnyAction;

        playerInputActions.Player.UpMotion.performed += OnAnyAction;
        playerInputActions.Player.UpMotion.canceled += OnAnyAction;

        playerInputActions.Player.Recall.performed += OnAnyAction;
        playerInputActions.Player.Recall.canceled += OnAnyAction;

        playerInputActions.Player.Pause.canceled += OnAnyAction;
        playerInputActions.PauseMenu.UnPause.canceled += OnAnyAction;

        playerInputActions.PauseMenu.ChangeMenuLeft.performed += OnAnyAction;
        playerInputActions.PauseMenu.ChangeMenuRight.performed += OnAnyAction;

        playerInputActions.PauseMenu.MoveMapUp.performed += OnAnyAction;
        playerInputActions.PauseMenu.MoveMapUp.performed += OnAnyAction;

        playerInputActions.Player.Movement.performed += OnAnyAction;
        playerInputActions.Player.AimPosition.performed += OnAnyAction;

        playerInput.onControlsChanged += OnControlSchemeChange;

        if(GameManagerScript.instance.isMainMenu)
        {
            return;
        }

        //playerInputActions.Player.RightMotion.performed += RightMotion_performed;
        //playerInputActions.Player.RightMotion.canceled += RightMotion_canceled;

        //playerInputActions.Player.LeftMotion.performed += LeftMotion_performed;
        //playerInputActions.Player.LeftMotion.canceled += LeftMotion_canceled;

        playerInputActions.Player.Dash.performed += Dash_performed;
        playerInputActions.Player.Dash.canceled += Dash_canceled;
    }

    public void ChangeInput(InputMode inputMode)
    {
        if(inputMode == InputMode.Game)
        {
            playerInputActions.Player.Enable();
            playerInputActions.PauseMenu.Disable();
            playerInputActions.NoInput.Disable();
        }

        if (inputMode == InputMode.Menu)
        {
            playerInputActions.Player.Disable();
            playerInputActions.PauseMenu.Enable();
            playerInputActions.NoInput.Disable();
        }

        if (inputMode == InputMode.None)
        {
            playerInputActions.Player.Disable();
            playerInputActions.PauseMenu.Disable();
            playerInputActions.NoInput.Enable();
        }

    }

    private void MoveMapUp(InputAction.CallbackContext obj)
    {
        if(mapMovementAvailable)
            GameManagerScript.instance.menuMap.MoveMap(0, 1);
    }

    private void MoveMapDown(InputAction.CallbackContext obj)
    {
        if (mapMovementAvailable)
            GameManagerScript.instance.menuMap.MoveMap(0, -1);
    }

    private void MoveMapLeft(InputAction.CallbackContext obj)
    {
        if (mapMovementAvailable)
            GameManagerScript.instance.menuMap.MoveMap(1, 0);
    }

    private void MoveMapRight(InputAction.CallbackContext obj)
    {
        if (mapMovementAvailable)
            GameManagerScript.instance.menuMap.MoveMap(-1, 0);
    }

    private void EnableMapMovement(InputAction.CallbackContext obj)
    {
        mapMovementAvailable = true;
    }

    private void DisableMapMovement(InputAction.CallbackContext obj)
    {
        mapMovementAvailable = false;
    }

    private void Dash_canceled(InputAction.CallbackContext obj)
    {

    }

    private void Dash_performed(InputAction.CallbackContext obj)
    {
        if(movementInput.x > 0)
        {
            characterController.RightDash();
        }
        if(movementInput.x < 0)
        {
            characterController.LeftDash();
        }
    }

    public override void Start()
    {
        base.Start();
        stoppedEvent.AddListener(OnStop);

        if (GameManagerScript.instance.isMainMenu)
            return;

        arrowCapacityIncreaseEvent.AddListener(OnArrowCapacityIncrease);
        arrowDamageIncreaseEvent.AddListener(OnArrowDamageIncrease);
        arrowReloadSpeedIncreaseEvent.AddListener(OnArrowReloadSpeedIncrease);

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

        ChangeAimReticle();

        //foreach (var item in reticleAndDirectionIndicators)
        //{
        //    item.SetActive(false);
        //}

        //if (playerInput.currentControlScheme == "Gamepad")
        //{
        //    reticleAndDirectionIndicators[0].SetActive(value);
        //    reticleAndDirectionIndicators[1].SetActive(!value);
        //}
        //else if (playerInput.currentControlScheme == "Keyboard")
        //{
        //    reticleAndDirectionIndicators[1].SetActive(value);
        //    reticleAndDirectionIndicators[0].SetActive(!value);
        //}

        playerData.unlockedBow = value;
    }



    private void OnAnyAction(InputAction.CallbackContext obj)
    {
        //currentControlScheme = obj.control.path;
    }

    private void LeftMotion_canceled(InputAction.CallbackContext obj)
    {
        
    }

    private void LeftMotion_performed(InputAction.CallbackContext obj)
    {
        characterController.LeftDash();        
    }

    private void RightMotion_canceled(InputAction.CallbackContext obj)
    {

    }

    private void RightMotion_performed(InputAction.CallbackContext obj)
    {
        characterController.RightDash();
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
        characterController.Jump();
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
        if(GameManagerScript.instance.pauseMenu == null)
        {
            return;
        }
        //Debug.Log(playerInput.currentActionMap.name);
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
        if (GameManagerScript.instance.pauseMenu == null)
        {
            return;
        }
        GameManagerScript.instance.pauseMenu.gameObject.SetActive(false);
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

    private void OnControlSchemeChange(PlayerInput input)
    {
        if (GameManagerScript.instance.isMainMenu)
            return;
        if (input.currentControlScheme == "Gamepad")
        {
            reticleAndDirectionIndicatorsChildren[0].SetActive(false);
            reticleAndDirectionIndicatorsChildren[1].SetActive(true); 
        }
        else if (input.currentControlScheme == "Keyboard")
        {
            reticleAndDirectionIndicatorsChildren[0].SetActive(true);
            reticleAndDirectionIndicatorsChildren[1].SetActive(false);
        }
    }

    private void ChangeAimReticle()
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            reticleAndDirectionIndicatorsChildren[0].SetActive(false);
            reticleAndDirectionIndicatorsChildren[1].SetActive(true);
        }
        else if (playerInput.currentControlScheme == "Keyboard")
        {
            reticleAndDirectionIndicatorsChildren[0].SetActive(true);
            reticleAndDirectionIndicatorsChildren[1].SetActive(false);
        }
    }

    void ReadInput()
    {
        movementInput = playerInputActions.Player.Movement.ReadValue<Vector2>();

        if (playerInput.currentControlScheme == "Gamepad")
        {
            aimInput = playerInputActions.Player.AimPositionGamepad.ReadValue<Vector2>();
            if (Mathf.Abs(aimInput.x) < 0.05f)
                aimInput.x = 0.05f * previousAimPosition.x;
            if (Mathf.Abs(aimInput.y) < 0.05f)
                aimInput.y = 0.05f * previousAimPosition.y;

            aimInputScreenPosition = new Vector3(transform.position.x + aimInput.x, transform.position.y + aimInput.y, 0f);

        }
        else if(playerInput.currentControlScheme == "Keyboard")
        {
            aimInput = playerInputActions.Player.AimPosition.ReadValue<Vector2>();
            aimInputScreenPosition = Camera.main.ScreenToWorldPoint(aimInput);
        }

        aimInputScreenPosition.z = Camera.main.nearClipPlane;
        previousAimPosition = aimInput;
    }

    public void OnSecretMapCollect()
    {
        CollectibleList collectibleList = GameManagerScript.instance.collectibleList;

        for (int i = 0; i < collectibleList.collectibles.Count; i++)
        {
            if (!progressTracker.collectibles.Exists(x => x.collectibleId == collectibleList.collectibles[i].collectibleId))
            {
                //Debug.LogWarning($"Manual MapPos X & Y: {collectibleList.collectibles[i].collectiblePosX + 25}, {collectibleList.collectibles[i].collectiblePosY + 25}");
                GameManagerScript.instance.minimap.ManuallySetMinimapTile(collectibleList.collectibles[i].collectiblePosX + 25, collectibleList.collectibles[i].collectiblePosY + 25, GameManagerScript.instance.mapArray[collectibleList.collectibles[i].collectiblePosX + 25, collectibleList.collectibles[i].collectiblePosY + 25], 3);
            }
        }
        GameManagerScript.instance.ManualMapRefresh();
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

    void OnArrowDamageIncrease()
    {
        playerData.damageModifier += 0.05f;
    }

    void OnArrowReloadSpeedIncrease()
    {
        playerData.reloadSpeedModifier -= 0.05f;
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
            Debug.LogWarning("Player has collected more arrow type upgrades than the ammount of available types");
        }
    }

    public void RefreshUI()
    {
        remainingHearthsScript.SetHearthImages(playerData.maxHealth);
        remainingArrowScript.OnGameLoad(playerData.maxArrowCount, playerWeaponSwap.unlockedWeapons);
        remainingHearthsScript.UpdateCurrentHealthOnImages((int)playerData.health);
    }

    void OnArrowChange(int changeDirection)
    {
        playerWeaponSwap.OnWeaponSwitch(changeDirection);
        remainingArrowScript.ChangeImagePrefab();
    }

    public void RestoreHealth(float healthRestored)
    {
        if(healable)
        {
            if (playerData.health >= playerData.maxHealth)
            {
                return;
            }
            HealthPickup(healthRestored);
            StartCoroutine(HealingTimer());
            GameStateManager.instance.audioManager.PlaySoundEffect(onHealthRestoreAudio);
        }
    }

    public override void TakeDamage(float damage)
    {
        if(isInvincible)
        {
            return;
        }
        if (damageable)
        {
            playerData.health -= damage;

            if(playerData.health <= 0)
            {
                GameStateManager.instance.audioManager.PlaySoundEffect(onDeathAudioClip);
                DeathScreen();
            }

            remainingHearthsScript.UpdateCurrentHealthOnImages((int)playerData.health);
            StartCoroutine(DamageTimer());
            GameStateManager.instance.audioManager.PlaySoundEffect(onHitAudioClip);
        }
    }

    private void DeathScreen()
    {
        GameManagerScript.instance.entitiesManager.EntitiesPauseState(true);
        stoppedEvent.Invoke(true);
        Invincibility(true);
        SetInputState(false);
        StartCoroutine(DeathAnimation());
    }

    IEnumerator DeathAnimation()
    {
        GameStateManager.instance.audioManager.RemoveAudio();
        for (int i = 0; i < 100; i++)
        {
            playerSpriteRenderer.material.SetFloat(deathID, i / 100f);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        GameManagerScript.instance.cameraMovement.blackout.DOFade(1, 1f).SetUpdate(true).OnComplete(() =>
        {
            deathScreen.SetActive(true);
            GameManagerScript.instance.cameraMovement.blackout.DOFade(0, 1f).SetUpdate(true);
        });
    }

    public void Invincibility(bool state)
    {
        if (state)
        {
            spriteRenderer.material.SetFloat(InvincibilityID, 1);
            isInvincible = true;
        }
        else
        {
            spriteRenderer.material.SetFloat(InvincibilityID, 0);
            isInvincible = false;
        }
    }

    public void SetInputState(bool state)
    {
        if(state)
            playerInputActions.Enable();
        else
            playerInputActions.Disable();
    }

    public void MaxHealthPickup()
    {
        remainingHearthsScript.AddHearthImage();
        playerData.maxHealth += 4;
        HealthPickup(4);
    }

    public void HealthPickup(float healthAmmount)
    {
        float tempHealth = playerData.health + healthAmmount;
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
        playerSpriteRenderer.material.SetFloat(flashID, 1);
        remainingHearthsScript.FlashHearths(0, true);
        yield return new WaitForSeconds(0.1f);
        playerSpriteRenderer.material.SetFloat(flashID, 0);
        remainingHearthsScript.FlashHearths(0, false);
        yield return new WaitForSeconds(additionalDamageTakenDelay);
        damageable = true;
    }

    IEnumerator HealingTimer()
    {
        healable = false;
        playerSpriteRenderer.material.SetFloat(flashHealingID, 1);
        remainingHearthsScript.FlashHearths(1, true);
        yield return new WaitForSeconds(0.1f);
        playerSpriteRenderer.material.SetFloat(flashHealingID, 0);
        remainingHearthsScript.FlashHearths(1, false);
        yield return new WaitForSeconds(additionalDamageTakenDelay);
        healable = true;
    }

    public void PlayAttackSoundEffect()
    {
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }
}
