using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDeleteButton : MonoBehaviour
{
    [SerializeField]
    private StartMenu startMenu;
    [SerializeField]
    private GameObject deleteConfirmationScreen;
    [SerializeField]
    private Button noButton;

    public void ConfirmationScreen(bool state)
    {
        deleteConfirmationScreen.SetActive(state);
        if(state)
        {
            noButton.Select();
        }
        else
        {
            startMenu.FocusOnSaveFile(GameStateManager.instance.GetComponent<SavingAndLoading>().currentSaveFile - 1);
        }
    }
    public void DeleteSaveFile()
    {
        deleteConfirmationScreen.SetActive(false);
        GameStateManager.instance.GetComponent<SavingAndLoading>().RemoveSaveFile(GameStateManager.instance.GetComponent<SavingAndLoading>().currentSaveFile);
        startMenu.DisableSaveFile(GameStateManager.instance.GetComponent<SavingAndLoading>().currentSaveFile - 1);
    }

}
