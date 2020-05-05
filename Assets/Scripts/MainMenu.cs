using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private TMP_Text versionText;

        [Header("Settings")]
        [SerializeField] private TMP_Text musicVolumeLabel;
        [SerializeField] private TMP_Text fxVolumeLabel;
        [SerializeField] private Button fullscreenButton;
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        private Resolution[] resolutions;
        private TMP_Text fullscreenText;
    
        void Start()
        {
            resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
            fullscreenText = fullscreenButton.GetComponentInChildren<TMP_Text>();

            // Set initial UI
            versionText.text = $"Version {Application.version}";
            RefreshSettings();
        }

        /// <summary>
        /// Start playing the game, either from the save data or from the beginning if no data exists.
        /// </summary>
        public void Play()
        {
            if(SaveLoad.SaveExists())
            {
                sceneTransition.LoadScene(4);
            }
            else
            {
                sceneTransition.LoadScene(2);
            }
        }

        #region Settings
    
        /// <summary>
        /// Save the current player prefs.
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.Save();
            Debug.Log("SETTINGS SAVED");
        }

        /// <summary>
        /// Modify the volume of the game's music.
        /// </summary>
        /// <param name="value">The value to increase or decrease the volume by.</param>
        public void ModifyMusicVolume(float value)
        {
            mixer.GetFloat("musicVolume", out float volume);

            // Modify current volume
            float modifiedVolume = Mathf.Clamp(volume + value, -80, 0);
            mixer.SetFloat("musicVolume", modifiedVolume);
            PlayerPrefs.SetFloat("musicVolume", modifiedVolume);

            // Update UI
            RefreshSettings();
        }

        /// <summary>
        /// Modify the volume of the game's sound effects.
        /// </summary>
        /// <param name="value">The value to increase or decrease the volume by.</param>
        public void ModifyFXVolume(float value)
        {
            mixer.GetFloat("fxVolume", out float volume);

            // Modify current volume
            float modifiedVolume = Mathf.Clamp(volume + value, -80, 0);
            mixer.SetFloat("fxVolume", modifiedVolume);
            PlayerPrefs.SetFloat("fxVolume", modifiedVolume);

            // Update UI
            RefreshSettings();
        }

        /// <summary>
        /// Toggle whether the game is in fullscreen or windowed mode.
        /// </summary>
        public void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        /// <summary>
        /// Set the screen's resolution.
        /// </summary>
        /// <param name="resolutionIndex"></param>
        public void SetResolution (int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            SaveSettings();
        }

        /// <summary>
        /// Update the UI to show the game's current settings.
        /// </summary>
        private void RefreshSettings()
        {
            // FIXME: Music volume displaying as 99.something
            // Update volume
            mixer.GetFloat("musicVolume", out float volume);
            musicVolumeLabel.text = $"MUSIC volume: {(volume + 80) * 1.25f}%";
            mixer.GetFloat("fxVolume", out volume);
            fxVolumeLabel.text = $"EFFECTS volume: {(volume + 80) * 1.25f}%";

            // Update fullscreen
            fullscreenText.text = Screen.fullScreen ? "Set to WINDOWED" : "Set to FULLSCREEN";

            // TODO: Change resolution settings to work with buttons
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
}