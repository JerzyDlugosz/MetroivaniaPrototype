using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenAnim : MonoBehaviour
{
    [SerializeField]
    private MinimapAnimation playerImage;
    [SerializeField]
    private Transform startTransform;
    [SerializeField]
    private Transform menuTransform;

    public void OnClick()
    {
        playerImage.ManuallyStartCoroutine();
        playerImage.transform.DOLocalMoveX(800, 2f).SetEase(Ease.Linear);
        startTransform.DOLocalMoveX(0, 2f);
        menuTransform.DOLocalMoveX(-1000, 2f).SetEase(Ease.Linear);
    }
}
