using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnTextAccept : UnityEvent { }

public class TextInGame : MonoBehaviour
{
    public OnTextAccept onTextAcceptEvent;

    [SerializeField]
    private GameObject inGameTextPrefab;

    private InGameTextManager inGameText;
    [SerializeField]
    private string onTriggerEnterText;
    [SerializeField]
    private string onUseText;

    [SerializeField]
    private bool isUseable;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (inGameText == null)
            {
                inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
            }
            inGameText.ClearText();
            inGameText.gameObject.SetActive(true);
            inGameText.SetText(onTriggerEnterText);
            inGameText.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

            inGameText.SetupAnimation(0.2f, 2f);
            inGameText.StartAnimation();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponentInParent<Player>().playerData.isDownButtonHeld)
            {
                if(isUseable)
                {
                    if (inGameText == null)
                    {
                        inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
                    }
                    inGameText.ClearText();
                    inGameText.SetText(onUseText);
                    inGameText.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                    inGameText.StartAnimation();

                    onTextAcceptEvent.Invoke();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (inGameText == null)
            {
                inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
            }
            inGameText.ClearText();
            inGameText.gameObject.SetActive(false);
            Destroy(inGameText.gameObject);
        }
    }
}
