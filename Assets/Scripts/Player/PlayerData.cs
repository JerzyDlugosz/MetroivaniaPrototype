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

    public float attackDelay = 0f;

    public PlayerStates playerState;

    public bool showDebugInfo;


    //Stats
    //public float health = 0f;
    //public float maxHealth = 0f;

}