using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnDestroyEvent : UnityEvent { }

[Serializable]
public class OnStoppedEvent : UnityEvent<bool> { }

public class BaseEntity : MonoBehaviour
{
    protected static readonly int flashID = Shader.PropertyToID("_FlashStrength");
    protected static readonly int colorID = Shader.PropertyToID("_BaseColor");
    protected static readonly int InvincibilityID = Shader.PropertyToID("_InvincibilityStrength");

    #region Events
    public OnDestroyEvent destroyEvent;
    public OnStoppedEvent stoppedEvent;
    #endregion

    #region Flags
    public bool inWater;
    public bool inLava;
	public bool inAir;
	public bool isGrounded;
	public bool onLadder;
    public bool isSlowed;
    public bool isStopped;
    public bool onFire;
    public bool onJumpPad;
    #endregion


    public SpriteRenderer spriteRenderer;
    private Coroutine burningCoroutine;

    [SerializeField]
    protected AudioClip onAttackAudioClip;
    [SerializeField]
    protected AudioClip onHitAudioClip;
    [SerializeField]
    protected AudioClip onDeathAudioClip;

    public virtual void Start()
    {
        GameManagerScript.instance.entitiesManager.AddEntity(this);
        destroyEvent.AddListener(() => GameManagerScript.instance.entitiesManager.RemoveEntity(this));
        destroyEvent.AddListener(() => Destroy(gameObject));
    }

    public void Burning(float damage, int burnTicks)
    {
        if (burningCoroutine != null)
        {
            StopCoroutine(burningCoroutine);
        }
        onFire = true;
        burningCoroutine = StartCoroutine(BurnTimer(burnTicks, damage));
    }

    protected IEnumerator BurnTimer(float burnTicks, float damage)
    {
        spriteRenderer.material.SetColor(colorID, Color.red);
        for (int i = 0; i < burnTicks; i++)
        {
            yield return new WaitForSeconds(0.5f);
            TakeDamage(damage);
        }
        spriteRenderer.material.SetColor(colorID, Color.white);
        onFire = false;
    }

    public virtual void TakeDamage(float damage)
    {

    }
}
