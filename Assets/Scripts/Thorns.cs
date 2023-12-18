using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnThornsTriggerEnterEvent : UnityEvent { }

public class Thorns : MonoBehaviour
{

    public OnThornsTriggerEnterEvent onThornsTriggerEnter;
    [SerializeField]
    private float damage;
    [SerializeField]
    private Transform onDamageTeleport;
    public bool teleportOnTriggerEnter = false;

    public bool isEnabled = true;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isEnabled)
        {
            if (!teleportOnTriggerEnter)
            {
                if (collision.CompareTag("Player"))
                {
                    collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(damage);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isEnabled)
        {
            if (teleportOnTriggerEnter)
            {
                if (collision.CompareTag("Player"))
                {
                    Player player = collision.GetComponentInParent<Player>();
                    player.damageTakenEvent.Invoke(damage);
                    OnTeleport(player);
                }
            }
        }
    }

    private void OnTeleport(Player player)
    {
        CameraMovement camera = Camera.main.GetComponent<CameraMovement>();
        player.transform.SetParent(null);
        player.characterController.StopMovement(true);
        camera.blackout.DOFade(1, 0.5f).OnComplete(() =>
        {
            player.mainCamera.GetComponent<CameraMovement>().SnapCameraPosition();
            camera.blackout.DOFade(0, 0.5f);
            player.characterController.ForceTransportPlayerToPosition(onDamageTeleport.position);
            player.characterController.StopMovement(false);
            onThornsTriggerEnter.Invoke();
        });
    }
}
