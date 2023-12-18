using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class OnMenuSwap : UnityEvent { }

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject ItemMenu;
    [SerializeField]
    private GameObject MapMenu;
    [SerializeField]
    private GameObject OptionsMenu;

    private List<GameObject> menus= new List<GameObject>();
    private int currentMenuIndex = 0;
    [SerializeField]
    private List<Image> navItems = new List<Image>();

    [SerializeField]
    public OnMenuSwap onMenuSwapEvent;

    [SerializeField]
    private List<TextMeshProUGUI> leftText;
    [SerializeField]
    private List<TextMeshProUGUI> rightText;

    [SerializeField]
    private GameObject keyboardMoveText;
    [SerializeField]
    private GameObject gamepadMoveText;

    [SerializeField]
    private AudioClip menuSwapSound;

    [SerializeField]
    private GameObject secretMapQuestionMark;

    private void Awake()
    {
        menus.Add(ItemMenu);
        menus.Add(MapMenu);
        menus.Add(OptionsMenu);

        onMenuSwapEvent.AddListener(() =>
        {
            GameStateManager.instance.audioManager.PlaySoundEffect(menuSwapSound);
        });
    }

    private void OnEnable()
    {
        OnControlSchemeChange(GameManagerScript.instance.player.playerInput);
        GameManagerScript.instance.player.playerInput.onControlsChanged += OnControlSchemeChange;

        if (GameManagerScript.instance.player.reachedSecret == true)
            secretMapQuestionMark.SetActive(true);
        else
            secretMapQuestionMark.SetActive(false);

    }

    private void OnDisable()
    {
        GameManagerScript.instance.player.playerInput.onControlsChanged -= OnControlSchemeChange;
    }

    private void OnControlSchemeChange(PlayerInput input)
    {
        if (input.currentControlScheme == "Gamepad")
        {
            foreach (var item in leftText)
            {
                item.text = "LB";
            }
            foreach (var item in rightText)
            {
                item.text = "RB";
            }
            keyboardMoveText.SetActive(false);
            gamepadMoveText.SetActive(true);
        }
        else
        {
            foreach (var item in leftText)
            {
                item.text = "Q";
            }
            foreach (var item in rightText)
            {
                item.text = "R";
            }
            keyboardMoveText.SetActive(true);
            gamepadMoveText.SetActive(false);
        }
    }

    public void SwitchMenu(int direction)
    {
        DisableMenu(menus[currentMenuIndex]);

        currentMenuIndex += direction;
        if (currentMenuIndex > menus.Count - 1)
        {
            currentMenuIndex = 0;
        }
        if(currentMenuIndex < 0) 
        {
            currentMenuIndex = menus.Count - 1;
        }
        NavBarHighlight();
        EnableMenu(menus[currentMenuIndex]);
    }

    private void NavBarHighlight()
    {
        for (int i = 0; i < navItems.Count; i++)
        {
            if (i == currentMenuIndex)
            {
                navItems[i].color = new Color(0, 1, 1, 1);
            }
            else
            {
                navItems[i].color = new Color(1, 1, 1, 1);
            }

        }
    }

    public void OnPauseScreenEnable()
    {
        menus[currentMenuIndex].GetComponent<CustomUIMenu>().OnMenuSwap();
    }

    public void EnableMenu(GameObject menu)
    {
        menu.SetActive(true);
        menu.GetComponent<CustomUIMenu>().OnMenuSwap();
    }

    public void DisableMenu(GameObject menu)
    {
        menu.SetActive(false);
    }
}
