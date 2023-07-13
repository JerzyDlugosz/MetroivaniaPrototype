using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button deleteButton;
    public void OnClick(int saveFileNumber)
    {
        GameStateManager.instance.GetComponent<SavingAndLoading>().currentSaveFile = saveFileNumber;
        startButton.interactable = true;
        deleteButton.interactable = true;
    }
}
