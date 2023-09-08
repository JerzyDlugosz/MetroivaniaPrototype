using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnTimeElapsed : UnityEvent { }

public class InGameTextManager : MonoBehaviour
{
    public FancyAnimatedText childAnimatedText;
    [SerializeField]
    TextMeshProUGUI textMesh;

    public OnTimeElapsed onTimeElapsed;

    private void Start()
    {
        onTimeElapsed.AddListener(DestroyObject);
    }

    public void SetupAnimation(float animationDistance, float animationDuration)
    {
        childAnimatedText.animationDistance = animationDistance;
        childAnimatedText.animationDuration = animationDuration;
    }

    public void SetText(string text)
    {
        textMesh.SetText(text);
        textMesh.ForceMeshUpdate(true);
        childAnimatedText.SpreadText(textMesh);
    }

    public void StartAnimation()
    {
        childAnimatedText.AnimateText();
    }

    public void ClearText()
    {
        textMesh.text = null;
        childAnimatedText.ClearText();
    }

    public void SetTimer(float seconds)
    {
        StartCoroutine(Timer(seconds));
    }

    private IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);
        ClearText();
        onTimeElapsed.Invoke();
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
