using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, ISelectHandler
{
    /// <summary>
    /// audio clip to play when user selects the button (through navigation)
    /// </summary>
    [SerializeField]
    private AudioClip onSelectAudioClip;
    /// <summary>
    /// audio clip to play when user clicks the button
    /// </summary>
    [SerializeField]
    private AudioClip onClickAudioClip;

    public void OnClick(Button target)
    {
        //GetComponent<Button>().Select();
        target.Select();
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(onClickAudioClip);
    }

    public void OnClickAudio()
    {
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(onClickAudioClip);
    }

    public void ExitAplication()
    {
        Application.Quit();
    }

    public void ChangeScene(int sceneNumber)
    {
        Time.timeScale = 1f;
        GameStateManager.instance.LoadScene(sceneNumber);
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(onSelectAudioClip);
    }
}
