using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicAudioSource;
    [SerializeField]
    private AudioSource effectsAudioSoruce;

    private AudioClip currentMusicAudioClip;

    [SerializeField, Range(0, 1)]
    private float volume;


    public void OnMapChange(Map currentMap)
    {
        if (currentMap.backgroundMusic == null)
        {
            Debug.LogWarning($"{currentMap.name} has no background music");
            musicAudioSource.Stop();
            currentMusicAudioClip = null;
            return;
        }

        if (currentMusicAudioClip == currentMap.backgroundMusic)
            return;

        currentMusicAudioClip = currentMap.backgroundMusic;
        PlayAudio(currentMusicAudioClip);
    }

    private void PlayAudio(AudioClip audioClip)
    {
        Debug.Log($"Playing {audioClip.name}");
        musicAudioSource.Stop();
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }

    private void Update()
    {
        musicAudioSource.volume = volume;
        effectsAudioSoruce.volume = volume;
    }

}
