using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private SceneTransition sceneTransition;
    [SerializeField] private TMP_Text versionText;

    [Header("Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    
    void Start()
    {
        versionText.text = $"Version {Application.version}";
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
        UpdateSettings();
    }

    /// <summary>
    /// Start playing the game, either from the save data or from the beginning if no data exists.
    /// </summary>
    public void Play()
    {
        if(SaveLoad.SaveExists())
        {
            sceneTransition.LoadScene(3);
        }
        else
        {
            sceneTransition.LoadScene(1);
        }
    }

    #region Settings
    
    // Save the current PlayerPrefs
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("FxVolume", fxSlider.value);
        PlayerPrefs.SetInt("Quality", qualityDropdown.value);
        if(fullscreenToggle.isOn)
            PlayerPrefs.SetInt("Fullscreen", 1);
        else
            PlayerPrefs.SetInt("Fullscreen", 0);
        for(int i = 0; i < resolutions.Length; i++)
            if(resolutions[i].Equals(Screen.currentResolution))
            {
                PlayerPrefs.SetInt("Resolution", i);
                break;
            }
        PlayerPrefs.Save();
        Debug.Log("SETTINGS SAVED");
    }

    // Set the volume of the game's music
    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("musicVolume", volume);
        SaveSettings();
    }

    // Set the volume of the game's sound effects
    public void SetFXVolume(float volume)
    {
        mixer.SetFloat("fxVolume", volume);
        SaveSettings();
    }

    // Set the game's graphics quality level
    public void SetQualityLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        SaveSettings();
    }

    // Set whether the game window is fullscreen
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        SaveSettings();
    }

    // Set the screen's resolution
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveSettings();
    }

    // Update the UI to show the current settings
    private void UpdateSettings()
    {
        // Update volume
        mixer.GetFloat("musicVolume", out float volume);
        musicSlider.value = volume;
        mixer.GetFloat("fxVolume", out volume);
        fxSlider.value = volume;

        // Update graphics quality
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        // Update fullscreen
        fullscreenToggle.isOn = Screen.fullScreen;

        // Update resolution
        int resolutionIndex = 0;
        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i];
            resolutionOptions.Add(resolution.width + " x " + resolution.height);
            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
                resolutionIndex = i;
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    #endregion
}
