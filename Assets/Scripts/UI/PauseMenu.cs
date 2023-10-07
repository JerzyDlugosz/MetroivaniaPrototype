using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
    public OnMenuSwap onMenuSwapEvent;

    private void Awake()
    {
        menus.Add(ItemMenu);
        menus.Add(MapMenu);
        menus.Add(OptionsMenu);
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

        EnableMenu(menus[currentMenuIndex]);
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
