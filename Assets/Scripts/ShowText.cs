using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShowText : MonoBehaviour
{
    [SerializeField]
    private string textToShow;

    [SerializeField]
    private GameObject inGameTextPrefab;
    public InGameTextManager inGameText;

    [SerializeField]
    private float timeBeforeAction;
    private void Awake()
    {
        if (inGameText == null)
        {
            inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
        }
    }

    public void TextShow()
    {
        inGameText.ClearText();
        inGameText.gameObject.SetActive(true);
        inGameText.SetText(textToShow);
        inGameText.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        inGameText.SetupAnimation(0.2f, 2f);
        inGameText.StartAnimation();
        inGameText.SetTimer(timeBeforeAction);
    }
}
