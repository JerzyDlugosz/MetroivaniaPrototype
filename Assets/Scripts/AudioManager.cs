using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource effectsAudioSoruce;

    private AudioClip currentMusicAudioClip;

    /// <summary>
    /// Zone music. Lists can contain loop or start + loop
    /// </summary>
    [SerializeField]
    private List<AudioClip> Zone1Music;
    [SerializeField]
    private List<AudioClip> Zone2Music;
    [SerializeField]
    private List<AudioClip> Zone3Music;
    [SerializeField]
    private List<AudioClip> MenuMusic;

    [Range(0, 1)]
    public float musicVolume;
    [Range(0, 1)]
    public float effectsVolume;


    private void Start()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.5f);
    }

    public void OnMapChange(Zone zone)
    {
        Debug.LogWarning("zone: " + zone);
        switch (zone)
        {
            case Zone.Zone1:
                PlayAudioFromList(Zone1Music);

                break;

            case Zone.Zone2:
                PlayAudioFromList(Zone2Music);

                break;

            case Zone.Zone3:
                PlayAudioFromList(Zone3Music);

                break;
            case Zone.Menu:
                PlayAudioFromList(MenuMusic);

                break;
        }
    }

    private void PlayAudioFromList(List<AudioClip> audioClips)
    {
        foreach (var item in audioClips)
        {
            if (currentMusicAudioClip == item)
                return;
        }

        currentMusicAudioClip = audioClips[0];
        if (audioClips.Count > 1)
        {
            musicAudioSource.Stop();
            musicAudioSource.PlayOneShot(audioClips[0]);

            musicAudioSource.clip = audioClips[1];
            musicAudioSource.PlayScheduled(AudioSettings.dspTime + audioClips[0].length);
        }
        else
        {
            PlayAudio(audioClips[0]);
        }
    }

    private void PlayAudio(AudioClip audioClip)
    {
        Debug.Log($"Playing {audioClip.name}");
        musicAudioSource.Stop();
        musicAudioSource.clip = audioClip;
        musicAudioSource.Play();
    }

    public void RemoveAudio()
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = null;
        currentMusicAudioClip = null;
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
