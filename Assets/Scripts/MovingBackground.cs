using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> movingBackgrounds1;
    [SerializeField]
    private List<GameObject> movingBackgrounds2;

    [SerializeField]
    private float background1MoveSpeed;
    [SerializeField]
    private float background2MoveSpeed;

    private bool isMoving = false;

    public void MovementState(bool state)
    {
        //isMoving = state;
        if (state)
            StartAnimation();
        else
            StopAnimation();
    }

    //void Update()
    //{
    //    if(isMoving)
    //    {
    //        foreach (var item in movingBackgrounds1)
    //        {
    //            item.transform.position -= new Vector3(0.01f * background1MoveSpeed, 0f, 0f);
    //        }
    //        foreach (var item in movingBackgrounds2)
    //        {
    //            item.transform.position -= new Vector3(0.01f * background2MoveSpeed, 0f, 0f);
    //        }
    //    }
    //}

    public void StartAnimation()
    {
        transform.DOLocalMoveX(-48, 30).SetEase(Ease.Linear).SetLoops(-1).SetUpdate(true);
    }

    public void StopAnimation()
    {
        transform.DOKill();
    }

    public void OnDestroy()
    {
        StopAnimation();
    }
}
