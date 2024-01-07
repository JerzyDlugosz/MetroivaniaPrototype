using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    private Transform CongratulationText;
    [SerializeField]
    private Transform ProgressScreen;

    [SerializeField]
    private TextMeshProUGUI ExploreText;
    [SerializeField]
    private TextMeshProUGUI ItemText;
    [SerializeField]
    private TextMeshProUGUI TotalText;

    private float[] totals;

    [SerializeField]
    private GameObject continueImage;
    [SerializeField]
    private TextMeshProUGUI tipText;

    private bool canClickOff = false;
    private bool isPressed = false;

    [SerializeField]
    private SpriteRenderer shatterSpriteRenderer;
    private bool secretActive;

    [SerializeField]
    private AudioClip VictoryMusic;
    [SerializeField]
    private AudioClip SecretVictoryMusic;

    private float secretTime = 11f;
    private float secretTimer = 0f;

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

        if(secretActive)
        {
            secretTimer += Time.unscaledDeltaTime;
            //Debug.Log(secretTimer);
            if(secretTimer >= secretTime)
            {
                EndGame();
                secretTimer = 0f;
            }
        }
    }

    public void StartAnim()
    {
        DOTween.KillAll(false);
        secretActive = GameManagerScript.instance.player.reachedSecret;

        SavingAndLoading savingAndLoading = GameStateManager.instance.savingAndLoading;
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);
        savingAndLoading.LoadGameFile(save);
        totals = savingAndLoading.LoadSaveFileProgress(save);
        CongratulationText.GetComponent<TextMeshProUGUI>().text = "The End...?";

        //Debug.Log($"Loaded stats: explore: {totals[0]}%, Item: {totals[1]}%, total: {totals[2]}%");


        if (totals[3] >= 99)
        {
            GameManagerScript.instance.player.reachedSecret = true;
            secretActive = true;
        }

        if(secretActive)
        {
            GameStateManager.instance.audioManager.ChangeAudio(SecretVictoryMusic);
        }
        else
        {
            GameStateManager.instance.audioManager.ChangeAudio(VictoryMusic);
        }

        gameObject.SetActive(true);
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
        for (int i = 0; i < 21; i++)
        {
           
            ExploreText.text = $"{Mathf.Lerp(0, totals[0], i / 20f):0}%";
            ItemText.text = $"{Mathf.Lerp(0, totals[1], i / 20f):0}%";
            TotalText.text = $"{Mathf.Lerp(0, totals[3], i / 20f):0}%";
            //Debug.Log($"explore: {Mathf.Lerp(0, totals[0], i / 20f):0}%, Item: {Mathf.Lerp(0, totals[1], i / 20f):0}%, total: {Mathf.Lerp(0, totals[3], i / 20f):0}%");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        tipText.gameObject.SetActive(true);


        if (secretActive)
        {
            float waitTime = 0;
            for (int i = 1; i < 1000; i++)
            {
                waitTime = Mathf.Pow(0.999f, i) - 0.9f;
                ExploreText.text = $"{100 + i/2}%";
                ItemText.text = $"{100 + i/2}%";
                TotalText.text = $"{100 + i}%";
                tipText.text += ".";
                yield return new WaitForSecondsRealtime(waitTime);
            }
        }
        else
        {
            tipText.text = "It's not about the destination, it's about the journey";

            //if (totals[0] != 100)
            //{
            //    tipText.text = "It's not about the destination, it's about the journey";
            //}
            //else
            //{
            //    if (totals[1] != 100)
            //    {
            //        tipText.text = "Look after the pennies and the pounds will look after themselves";
            //    }
            //    else
            //    {
            //        if (totals[2] != 80)
            //        {
            //            tipText.text = "Contact the developer FAST!";
            //        }
            //    }
            //}

            continueImage.SetActive(true);
            continueImage.GetComponent<MinimapAnimation>().RestartCoroutine();
            canClickOff = true;
        }
    }

    void EndGame()
    {
        if(secretActive)
        {
            Secret();
            return;
        }
        ChangeScene(1);

    }

    public void ChangeScene(int sceneNumber)
    {
        Time.timeScale = 1f;
        GameStateManager.instance.LoadScene(sceneNumber);
    }

    void Secret()
    {
        GameManagerScript.instance.isMinimapAvailable = false;
        //Debug.Log("Secret!");
        Time.timeScale = 1f;
        GameManagerScript.instance.player.ChangeInput(InputMode.Game);
        GameManagerScript.instance.player.characterController.StopMovement(false);
        GameManagerScript.instance.entitiesManager.EntitiesPauseState(false);
        GameManagerScript.instance.ManualChangeTilemap(6, 15);
        GameManagerScript.instance.cameraMovement.SetBlackout(1);
        GameManagerScript.instance.minimap.MinimapState(false);
        gameObject.SetActive(false);
        //shatterSpriteRenderer.gameObject.SetActive(true);
        //shatterSpriteRenderer.material.SetFloat()
    }
}
