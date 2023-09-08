using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnDestroyEvent : UnityEvent { }


public class BaseEntity : MonoBehaviour
{
    protected static readonly int flashID = Shader.PropertyToID("_FlashStrength");
    protected static readonly int colorID = Shader.PropertyToID("_BaseColor");
        protected static readonly int InvincibilityID = Shader.PropertyToID("_InvincibilityStrength");

    #region Events
    public OnDestroyEvent destroyEvent;
    #endregion

    #region Flags
    public bool inWater;
	public bool inAir;
	public bool isGrounded;
	public bool onLadder;
    public bool isSlowed;
    #endregion
}
