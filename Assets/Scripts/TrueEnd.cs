using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TrueEnd : MonoBehaviour
{

    [SerializeField]
    private Transform CongratulationText;
    [SerializeField]
    private Transform ProgressScreen;
    private float[] totals;

    [SerializeField]
    private GameObject continueImage;
    [SerializeField]
    private TextMeshProUGUI tipText;

    private bool canClickOff = false;
    private bool isPressed = false;

    [SerializeField]
    private AudioClip VictoryMusic;

    //private void Start()
    //{
    //    StartAnim();
    //}

    void Update()
    {
        if (canClickOff)
        {
            if (!isPressed)
            {
                InputSystem.onAnyButtonPress
                    .CallOnce(ctrl => EndGame());
                isPressed = true;
            }
        }
    }

    public void StartAnim()
    {
        SavingAndLoading savingAndLoading = GameStateManager.instance.savingAndLoading;
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);
        savingAndLoading.LoadGameFile(save);
        totals = savingAndLoading.LoadSaveFileProgress(save);
        gameObject.SetActive(true);
        CongratulationText.GetComponent<TextMeshProUGUI>().text = "True End";
        Camera.main.GetComponent<CameraMovement>().blackout.DOFade(0, 1f).SetUpdate(true).OnComplete(() =>
        {
            StartCoroutine(StartEndScreenAnim());
        });
    }

    IEnumerator StartEndScreenAnim()
    {
        yield return new WaitForSecondsRealtime(1f);
        CongratulationText.DOLocalMoveY(125f, 1f).SetUpdate(true);
        ProgressScreen.DOLocalMoveY(0, 1f).SetUpdate(true).OnComplete(() =>
        {
            VisualCalculations();
        });
    }

    private void VisualCalculations()
    {
        StartCoroutine(TimedCalculations());
    }

    IEnumerator TimedCalculations()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        tipText.gameObject.SetActive(true);


        tipText.text = "Thanks for playing!";

        continueImage.SetActive(true);
        continueImage.GetComponent<MinimapAnimation>().RestartCoroutine();
        canClickOff = true;
    }

    void EndGame()
    {
        GameManagerScript.instance.OnTrueEndingReached();


        ChangeScene(1);
    }

    public void ChangeScene(int sceneNumber)
    {
        Time.timeScale = 1f;
        GameStateManager.instance.LoadScene(sceneNumber);
    }
}
