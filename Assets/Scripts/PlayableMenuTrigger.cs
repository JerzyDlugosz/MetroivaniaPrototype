using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMenuTrigger : MonoBehaviour
{
    private PlayableMenuUIManager playableMenuUIManager;
    public MenuType menuType;

    private bool usedTrigger;

    [SerializeField]
    private GameObject inGameTextPrefab;

    private InGameTextManager inGameText;
    [SerializeField]
    private string onTriggerStayText;
    [SerializeField]
    private string onSavedText;

    private void Start()
    {
        playableMenuUIManager = FindObjectOfType<PlayableMenuUIManager>();

        switch (menuType)
        {
            case MenuType.Start:
                playableMenuUIManager.startMenu.backButton.onClick.AddListener(() => { SetScreen(false); });

                break;
            case MenuType.Options:
                playableMenuUIManager.OptionsMenu.backButton.onClick.AddListener(() => { SetScreen(false); });
                break;
            default:
                break;
        }
    }

    private void SetScreen(bool state)
    {
        if (state)
            GameManagerScript.instance.player.playerInputActions.Disable();
        else
            GameManagerScript.instance.player.playerInputActions.Enable();

        switch (menuType)
        {
            case MenuType.Start:
                playableMenuUIManager.startMenu.gameObject.SetActive(state);
                
                if(state)
                    playableMenuUIManager.startMenu.FocusOnSaveFile(0);

                break;
            case MenuType.Options:
                playableMenuUIManager.OptionsMenu.gameObject.SetActive(state);
                
                if(state)
                    playableMenuUIManager.OptionsMenu.FocusOnButton();
                break;
            default:
                break;
        }
    }



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
            inGameText.SetText(onTriggerStayText);
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
                if (!usedTrigger)
                {
                    if (inGameText == null)
                    {
                        inGameText = Instantiate(inGameTextPrefab).GetComponent<InGameTextManager>();
                    }
                    inGameText.ClearText();
                    inGameText.SetText(onSavedText);
                    inGameText.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                    inGameText.StartAnimation();

                    SetScreen(true);

                    usedTrigger = true;
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

            SetScreen(false);
            usedTrigger = false;
        }
    }


}

public enum MenuType
{
    Start,
    Options
}
