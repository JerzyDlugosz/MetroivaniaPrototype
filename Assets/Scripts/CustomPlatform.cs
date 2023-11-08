using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomPlatform : MonoBehaviour
{
    private PlatformEffector2D platformEffector2D;

    private void Start()
    {
        platformEffector2D = GetComponent<PlatformEffector2D>();
        if (GameManagerScript.instance.player.playerInputActions != null)
        {
            GameManagerScript.instance.player.playerInputActions.Player.DownMotion.performed += DisablePlatformEffector;
            GameManagerScript.instance.player.playerInputActions.Player.DownMotion.canceled += EnablePlatformEffector;
        }
    }

    public void DisablePlatformEffector(InputAction.CallbackContext obj)
    {

        platformEffector2D.colliderMask &= ~(1 << 3);
        
    }

    public void EnablePlatformEffector(InputAction.CallbackContext obj)
    {

        platformEffector2D.colliderMask |= (1 << 3);
        
    }

    private void OnDestroy()
    {
        GameManagerScript.instance.player.playerInputActions.Player.DownMotion.performed -= DisablePlatformEffector;
        GameManagerScript.instance.player.playerInputActions.Player.DownMotion.canceled -= EnablePlatformEffector;
    }
}
