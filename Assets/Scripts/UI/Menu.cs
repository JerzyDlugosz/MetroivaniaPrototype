using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private Transform menu;
    [SerializeField]
    private AudioClip menuMusic;

    private void Start()
    {
        GameStateManager.instance.audioManager.ChangeAudio(menuMusic);
    }

    public void MoveMenuPosition(Transform targetPosition)
    {
        Debug.Log($"ThisPos {menu.localPosition}, targetPos {targetPosition.localPosition}");
        menu.DOLocalMove(-targetPosition.localPosition, 1f);
    }
}
