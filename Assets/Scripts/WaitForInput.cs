using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

[Serializable]
public class OnInputPressedEvent : UnityEvent { }
public class WaitForInput : MonoBehaviour
{
    public OnInputPressedEvent inputPressedEvent;
    bool isPressed = false;
    public Button button;
    private void Start()
    {
        GameManagerScript.instance.player.stoppedEvent.Invoke(true);
        GameManagerScript.instance.player.isStopped = true;

        inputPressedEvent.AddListener(() => Invoke(nameof(DelayedSelect), 0.1f));
        inputPressedEvent.AddListener(() => EnablePlayerMovement());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPressed)
        {
            InputSystem.onAnyButtonPress
                .CallOnce(ctrl => inputPressedEvent.Invoke());
            isPressed = true;
        }
    }

    void EnablePlayerMovement()
    {
        GameManagerScript.instance.player.stoppedEvent.Invoke(false);
        GameManagerScript.instance.player.isStopped = false;
    }

    void DelayedSelect()
    {
        button.Select();
    }
}
