using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource effectsAudioSoruce;

    private AudioClip currentMusicAudioClip;

    [Range(0, 1)]
    public float musicVolume;
    [Range(0, 1)]
    public float effectsVolume;


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

    public void ChangeAudio(AudioClip audioClip)
    {
        PlayAudio(audioClip);
    }

    public void PlaySoundEffect(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("No audio clip to play");
            return;
        }
        Debug.Log($"Playing: {audioClip.name}");
        effectsAudioSoruce.PlayOneShot(audioClip, effectsVolume);
    }

    private void Update()
    {
        musicAudioSource.volume = musicVolume;
        effectsAudioSoruce.volume = effectsVolume;
    }

}
