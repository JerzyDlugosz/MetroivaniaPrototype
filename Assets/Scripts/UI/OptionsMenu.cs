using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : CustomUIMenu
{
    [SerializeField]
    private TMP_Dropdown ResolutionDropdown;
    [SerializeField]
    private List<Vector2Int> resolutions = new List<Vector2Int>();

    //[SerializeField]
    //private Toggle FullscreenToggle;

    [SerializeField]
    private TMP_Dropdown FrameRateDropdown;


    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider effectsVolumeSlider;

    private void Start()
    {
        SetUIValues();

        ResolutionDropdown.onValueChanged.AddListener(delegate {
            OnResolutionChange(ResolutionDropdown);
        });

        FrameRateDropdown.onValueChanged.AddListener(delegate {
            SetTargetFrameRate(FrameRateDropdown);
        });

        //FullscreenToggle.onValueChanged.AddListener(delegate {
        ////    SetFullscreen(FullscreenToggle);
        ////});

        musicVolumeSlider.onValueChanged.AddListener(delegate {
            OnMusicVolumeSliderChange(musicVolumeSlider);
        });

        effectsVolumeSlider.onValueChanged.AddListener(delegate {
            OnEffectsVolumeSliderChange(effectsVolumeSlider);
        });

    }

    private void SetUIValues()
    {
        //int fullscreen = PlayerPrefs.GetInt("FullScreen", 0);

#if UNITY_STANDALONE_WIN

            int res = PlayerPrefs.GetInt("Resolution", 1);
            ResolutionDropdown.value = res;

#endif

        int fps = PlayerPrefs.GetInt("FrameRate", 1);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 0.5f);

        //if (fullscreen == 1)
        //{
        //    FullscreenToggle.isOn = true;
        //}
        //else
        //{
        //    FullscreenToggle.isOn = false;
        //}

        FrameRateDropdown.value = fps;
        musicVolumeSlider.value = musicVolume;
        effectsVolumeSlider.value = effectsVolume;
    }

    public void OnResolutionChange(TMP_Dropdown dropdown)
    {
        Screen.SetResolution(resolutions[dropdown.value].x, resolutions[dropdown.value].y, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", dropdown.value);
    }

    //public void SetFullscreen(Toggle toggle)
    //{
    //    if (toggle.isOn)
    //    {
    //        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    //        PlayerPrefs.SetInt("FullScreen", 1);
    //    }
    //    else
    //    {
    //        Screen.fullScreenMode = FullScreenMode.Windowed;
    //        PlayerPrefs.SetInt("FullScreen", 0);
    //    }
    //}

    public void SetTargetFrameRate(TMP_Dropdown dropdown)
    {
        QualitySettings.vSyncCount = dropdown.value;
        PlayerPrefs.SetInt("FrameRate", dropdown.value);
    }

    public void OnMusicVolumeSliderChange(Slider slider)
    {
        GameStateManager.instance.audioManager.musicVolume = slider.value;
        PlayerPrefs.SetFloat("MusicVolume", slider.value);
    }

    public void OnEffectsVolumeSliderChange(Slider slider)
    {
        GameStateManager.instance.audioManager.effectsVolume = slider.value;
        PlayerPrefs.SetFloat("EffectsVolume", slider.value);
    }

    public override void OnMenuSwap()
    {
        musicVolumeSlider.Select();
    }
}
