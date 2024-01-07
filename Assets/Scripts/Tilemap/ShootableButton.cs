using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnButtonHitEvent : UnityEvent { }

public class ShootableButton : MonoBehaviour
{
    public OnButtonHitEvent onButtonHit;
    public AudioClip onHitAudioClip;

    private void Start()
    {
        onButtonHit.AddListener(DebugMessage);
    }

    private void DebugMessage()
    {
        Debug.Log("Hit the button");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerProjectile projectile))
        {
            //projectile.wallCollisionEvent.Invoke();
            projectile.destroyEvent.Invoke();
            GameStateManager.instance.audioManager.PlaySoundEffect(onHitAudioClip);
            onButtonHit.Invoke();
        }
    }
}
