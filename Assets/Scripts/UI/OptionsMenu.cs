using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : CustomUIMenu
{
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider effectsVolumeSlider;

    public override void OnMenuSwap()
    {
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        musicVolumeSlider.value = GameStateManager.instance.audioManager.musicVolume;
        effectsVolumeSlider.value = GameStateManager.instance.audioManager.effectsVolume;
    }

    public void OnMusicVolumeSliderChange()
    {
        GameStateManager.instance.audioManager.musicVolume = musicVolumeSlider.value;
    }

    public void OnEffectsVolumeSliderChange()
    {
        GameStateManager.instance.audioManager.effectsVolume = effectsVolumeSlider.value;
    }
}
