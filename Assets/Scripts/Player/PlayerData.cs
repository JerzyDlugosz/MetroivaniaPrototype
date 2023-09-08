using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public Vector3 motion;
    public Vector3 animationMotion;

    public bool isRunning = false;
    public bool isShooting = false;
    public bool isJumping = false;
    public bool isDownButtonHeld = false;
    public bool isUpButtonHeld = false;
    public bool isRecallButtonHeld = false;

    public float attackDelay = 0f;
    public float baseGravityScale = 6f;
    public float baseJumpSpeed = 1400f;
    public float baseMovementSpeed = 3000f;

    public PlayerStates playerState;

    public bool showDebugInfo;

    [SerializeField]
    private bool UnlockedBow = false;
    public bool unlockedBow
    {
        get { return UnlockedBow; }
        set
        {
            if (value == UnlockedBow)
                return;

            UnlockedBow = value;
            GetComponent<Player>().OnBowUnlocked(value);
        }
    }
    #region Stats
    public int health = 0;
    public int maxHealth = 0;

    public byte currentArrowCount = 0;
    public byte maxArrowCount = 0;
    #endregion

    #region Collectibles
    public bool WaterSpirit = false;
    public bool EarthSpirit = false;
    #endregion
}