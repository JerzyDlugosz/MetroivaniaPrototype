using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomPlatformParent : MonoBehaviour
{
    private Transform colliders;
    private List<PlatformEffector2D> platformEffectors = new List<PlatformEffector2D>();

    private void Start()
    {
        if(colliders == null) 
        {
            colliders = transform.GetChild(0);
        }


        if(colliders.childCount == 0)
        {
            return;
        }

        foreach (Transform child in colliders)
        {
            platformEffectors.Add(child.GetComponent<PlatformEffector2D>());
        }

        if(GameManagerScript.instance.player.playerInputActions != null)
        {
            GameManagerScript.instance.player.playerInputActions.Player.DownMotion.performed += DisablePlatformEffector;
            GameManagerScript.instance.player.playerInputActions.Player.DownMotion.canceled += EnablePlatformEffector;
        }
    }

    public void DisablePlatformEffector(InputAction.CallbackContext obj)
    {
        foreach (var item in platformEffectors)
        {
            item.colliderMask &= ~(1 << 3);
        }
    }

    public void EnablePlatformEffector(InputAction.CallbackContext obj)
    {
        foreach (var item in platformEffectors)
        {
            item.colliderMask |= (1 << 3);
        }
    }

    public void SetColliderState(bool state)
    {
        colliders.gameObject.SetActive(state);
    }

    private void OnDestroy()
    {
        GameManagerScript.instance.player.playerInputActions.Player.DownMotion.performed -= DisablePlatformEffector;
        GameManagerScript.instance.player.playerInputActions.Player.DownMotion.canceled -= EnablePlatformEffector;
    }
}
