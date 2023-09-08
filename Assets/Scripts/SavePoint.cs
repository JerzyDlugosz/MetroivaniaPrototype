using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool isSaving;

    [SerializeField]
    private GameObject inGameTextPrefab;

    private InGameTextManager inGameText;
    [SerializeField]
    private string onTriggerStayText;
    [SerializeField]
    private string onSavedText;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(inGameText == null)
            {
                inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
            }
            Debug.Log("Entered saving zone");
            inGameText.ClearText();
            inGameText.gameObject.SetActive(true);
            inGameText.SetText(onTriggerStayText);
            inGameText.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

            inGameText.SetupAnimation(0.2f, 2f);
            inGameText.StartAnimation();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(collision.GetComponentInParent<Player>().playerData.isDownButtonHeld)
            {
                if (!isSaving)
                {
                    if (inGameText == null)
                    {
                        inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
                    }
                    inGameText.ClearText();
                    inGameText.SetText(onSavedText);
                    inGameText.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                    inGameText.StartAnimation();
                    GameManagerScript.instance.SaveGame();
                    isSaving = true;
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
            Debug.Log("Exited saving zone");
            inGameText.ClearText();
            inGameText.gameObject.SetActive(false);
            Destroy(inGameText.gameObject);
        }

        isSaving = false;
    }
}
