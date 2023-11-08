using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public Image blackout;
    public void StartGame()
    {
        blackout.DOFade(1, 2f).OnComplete(() => {
            GameStateManager.instance.LoadGameSceneWithLoadingScreen();
        });
    }
}
