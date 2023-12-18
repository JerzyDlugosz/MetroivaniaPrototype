using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScreen : MonoBehaviour
{

    [SerializeField]
    private GameObject CreditsText;
    [SerializeField]
    private GameObject movingBackground;
    [SerializeField]
    private GameObject platform;

    public void ShowCredits()
    {
        GameManagerScript.instance.player.ChangeInput(InputMode.Menu);

        //Saving doesnt work correctly. It seems that saving a map thats not the current map breaks loading

        //SavingAndLoading savingAndLoading = GameStateManager.instance.savingAndLoading;
        //Save save = savingAndLoading.GetSaveFile(savingAndLoading.currentSaveFile);
        //GameManagerScript.instance.SaveGame(new Vector2(7, 15));
        //savingAndLoading.SaveGameFile(save);

        movingBackground.transform.DOLocalMoveY(48, 30).SetEase(Ease.Linear);
        CreditsText.transform.DOLocalMoveY(750, 25).SetEase(Ease.Linear).OnComplete(() =>
        {
            platform.SetActive(false);
        });
    }

    public void StartEndScreen()
    {
        GameManagerScript.instance.trueEndScreen.StartAnim();
    }

}
