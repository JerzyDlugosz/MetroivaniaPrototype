using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

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

    //private void Start()
    //{
    //    StartAnim();
    //}

    public void StartAnim()
    {
        gameObject.SetActive(true);
        Camera.main.GetComponent<CameraMovement>().blackout.DOFade(0, 1f).OnComplete(() =>
        {
            Invoke("StartEndScreenAnim", 1f);
        });
    }

    private void StartEndScreenAnim()
    {
        CongratulationText.DOLocalMoveY(125f, 1f);
        ProgressScreen.DOLocalMoveY(0, 1f).OnComplete(() =>
        {
            VisualCalculations();
        });
    }

    private void VisualCalculations()
    {
        SavingAndLoading savingAndLoading = GameStateManager.instance.savingAndLoading;
        Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);
        savingAndLoading.LoadGameFile(save);
        totals = savingAndLoading.LoadSaveFileProgress(save);

        StartCoroutine(TimedCalculations());
    }

    IEnumerator TimedCalculations()
    {
        for (int i = 0; i < 20; i++)
        {
           
            ExploreText.text = $"{Mathf.Lerp(0, totals[0], i/20f)}%";
            ItemText.text = $"{Mathf.Lerp(0, totals[1], i / 20f)}%";
            TotalText.text = $"{Mathf.Lerp(0, totals[3], i / 20f)}%";
            Debug.Log($"explore: {Mathf.Lerp(0, totals[0], i / 20f)}%, Item: {Mathf.Lerp(0, totals[1], i / 20f)}%, total: {Mathf.Lerp(0, totals[3], i / 20f)}%");
            yield return new WaitForSeconds(0.1f);
        }
    }
}
