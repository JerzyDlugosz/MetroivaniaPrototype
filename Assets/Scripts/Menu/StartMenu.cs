using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public Button startButton;
    public Button deleteButton;
    public Button backButton;

    private TextMeshProUGUI startText;
    private TextMeshProUGUI deleteText;

    private SavingAndLoading savingAndLoading;

    [SerializeField]
    private List<SaveFileUIObject> saveFiles;

    private bool[] usedSaveFiles = new bool[3];

    [SerializeField]
    private List<GameObject> saveFileHighligths;

    private bool setFileNav = false;

    [SerializeField]
    private List<String> zoneNames;

    public void Start()
    {
        savingAndLoading = GameStateManager.instance.GetComponent<SavingAndLoading>();

        startText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        deleteText = deleteButton.GetComponentInChildren<TextMeshProUGUI>();


        for (int i = 0; i < 3; i++)
        {
            Save save = savingAndLoading.GetSaveFile(i + 1);
            if (savingAndLoading.CheckIfSaveFileExists(save))
            {
                savingAndLoading.LoadGameFile(save);
                float[] totals = savingAndLoading.LoadSaveFileProgress(save);

                if (savingAndLoading.loadTrueEndingState(save))
                {
                    saveFiles[i].trueEndPanel.SetActive(true);

                    saveFiles[i].savePanel.SetActive(false);
                    saveFiles[i].noSavePanel.SetActive(false);

                    var ts = TimeSpan.FromSeconds(save.timePlayed);

                    saveFiles[i].trueEndPlayTime.text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
                }
                else
                {
                    saveFiles[i].trueEndPanel.SetActive(false);

                    saveFiles[i].savePanel.SetActive(true);
                    saveFiles[i].noSavePanel.SetActive(false);

                    saveFiles[i].zoneName.text = zoneNames[(int)save.zone];

                    var ts = TimeSpan.FromSeconds(save.timePlayed);

                    saveFiles[i].playTime.text = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
                    saveFiles[i].mapCompletedNumber.text = $"{totals[0]:0}%";
                    saveFiles[i].itemsCollectedNumber.text = $"{totals[1]:0}%";
                    saveFiles[i].bossesKilledNumber.text = $"{totals[2]:0}%";

                }

                usedSaveFiles[i] = true;
            }
            else
            {
                saveFiles[i].savePanel.SetActive(false);
                saveFiles[i].noSavePanel.SetActive(true);

                usedSaveFiles[i] = false;
            }
        }
    }

    public void DisableSaveFile(int saveFileNumber)
    {
        saveFiles[saveFileNumber].savePanel.SetActive(false);
        saveFiles[saveFileNumber].noSavePanel.SetActive(true);

        usedSaveFiles[saveFileNumber] = false;

        FocusOnSaveFile(saveFileNumber);
        OnClick(saveFileNumber + 1);
    }

    public void OnClick(int saveFileNumber)
    {
        savingAndLoading.currentSaveFile = saveFileNumber;
        startButton.interactable = true;
        startText.color = new Color(startText.color.r, startText.color.g, startText.color.b, 1f);

        foreach (var item in saveFiles)
        {
            item.background.color = new Color(1f, 1f, 1f, 1f);
        }

        saveFiles[saveFileNumber - 1].background.color = new Color(0, 1f, 0, 1f);

        if(!setFileNav)
        {
            SetSaveFileNavigation();
            setFileNav = true;
        }


        if (usedSaveFiles[saveFileNumber - 1])
        {
            startText.text = "Continue";
            deleteButton.interactable = true;
            deleteText.color = new Color(deleteText.color.r, deleteText.color.g, deleteText.color.b, 1f);

            ChangeNavigation(saveFileNumber - 1, false);
        }
        else
        {
            startText.text = "Start";
            deleteButton.interactable = false;
            deleteText.color = new Color(deleteText.color.r, deleteText.color.g, deleteText.color.b, 0.5f);

            ChangeNavigation(saveFileNumber - 1, true);
        }
    }

    private void ChangeNavigation(int navNumber, bool skipDeleteButton)
    {
        Navigation backButtonNav = backButton.navigation;
        Navigation startButtonNav = startButton.navigation;

        backButtonNav.selectOnDown = saveFiles[navNumber].GetComponent<Button>();
        startButtonNav.selectOnUp = saveFiles[navNumber].GetComponent<Button>();

        if (!skipDeleteButton)
        {
            backButtonNav.selectOnUp = deleteButton.GetComponent<Button>();
            startButtonNav.selectOnDown = deleteButton.GetComponent<Button>();
        }
        else
        {
            backButtonNav.selectOnUp = startButton.GetComponent<Button>();
            startButtonNav.selectOnDown = backButton.GetComponent<Button>();
        }


        backButton.navigation = backButtonNav;
        startButton.navigation = startButtonNav;
    }

    public void FocusOnSaveFile(int saveFileNumber)
    {
        saveFiles[saveFileNumber].GetComponent<Button>().Select();
    }

    private void SetSaveFileNavigation()
    {
        Navigation saveFiles1 = saveFiles[0].GetComponent<Button>().navigation;
        Navigation saveFiles2 = saveFiles[1].GetComponent<Button>().navigation;
        Navigation saveFiles3 = saveFiles[2].GetComponent<Button>().navigation;

        saveFiles1.selectOnDown = startButton.GetComponent<Button>();
        saveFiles2.selectOnDown = startButton.GetComponent<Button>();
        saveFiles3.selectOnDown = startButton.GetComponent<Button>();

        saveFiles1.selectOnUp = backButton.GetComponent<Button>();
        saveFiles2.selectOnUp = backButton.GetComponent<Button>();
        saveFiles3.selectOnUp = backButton.GetComponent<Button>();

        saveFiles[0].GetComponent<Button>().navigation = saveFiles1;
        saveFiles[1].GetComponent<Button>().navigation = saveFiles2;
        saveFiles[2].GetComponent<Button>().navigation = saveFiles3;
    }

    //public void SetHighlights(int highlightNumber)
    //{
    //    foreach (var item in saveFileHighligths)
    //    {
    //        item.SetActive(false);
    //    }
    //    saveFileHighligths[highlightNumber].SetActive(true);
    //    saveFileHighligths[highlightNumber].GetComponent<MinimapAnimation>().RestartCoroutine();
    //}
}
