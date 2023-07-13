using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private Transform menu;

    public void MoveMenuPosition(Transform targetPosition)
    {
        Debug.Log($"ThisPos {menu.localPosition}, targetPos {targetPosition.localPosition}");
        menu.DOLocalMove(-targetPosition.localPosition, 1f);
    }
}
