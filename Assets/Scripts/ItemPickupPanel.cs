using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class ItemPickupPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemNameText;
    [SerializeField]
    private TextMeshProUGUI itemDescriptionText;
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private GameObject continueImage;

    private bool canClickOff = false;
    private bool isPressed = false;

    public static ItemPickupPanel instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(canClickOff)
        {
            if (!isPressed)
            {
                InputSystem.onAnyButtonPress
                    .CallOnce(ctrl => HidePanel());
                isPressed = true;
            }
        }
    }

    public void ShowPanel(string itemName, string itemDescription, Sprite itemSprite)
    {
        GameManagerScript.instance.player.ChangeInput(InputMode.Menu);
        GameManagerScript.instance.entitiesManager.EntitiesPauseState(true);
        FindObjectOfType<Player>().stoppedEvent.Invoke(true);

        transform.GetChild(0).gameObject.SetActive(true);
        itemNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemImage.sprite = itemSprite;
        transform.GetChild(0).DOScaleY(1, 1f).SetUpdate(true).OnComplete(() => 
        {
            StartCoroutine(DelayedAction());
        });
    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        continueImage.SetActive(true);
        continueImage.GetComponent<MinimapAnimation>().RestartCoroutine();
        canClickOff = true;
    }

    private void HidePanel()
    {
        canClickOff = false;
        isPressed = false;
        continueImage.SetActive(false);

        transform.GetChild(0).DOScaleY(0, 1f).SetUpdate(true).OnComplete(() => 
        {
            transform.GetChild(0).gameObject.SetActive(false);

            GameManagerScript.instance.entitiesManager.EntitiesPauseState(false);
            FindObjectOfType<Player>().stoppedEvent.Invoke(false);
            GameManagerScript.instance.player.ChangeInput(InputMode.Game);
        });
    }
}
