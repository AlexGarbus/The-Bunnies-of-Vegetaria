using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private SceneTransition sceneTransition;

        [Header("Settings")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private TMP_Text musicVolumeLabel;
        [SerializeField] private TMP_Text fxVolumeLabel;
        [SerializeField] private Button fullscreenButton;
        [SerializeField] private TMP_Text resolutionText;
        [SerializeField] private TMP_Text versionText;
        [Tooltip("Game objects for settings that should only be visible when playing on PC, Mac, or Linux.")]
        [SerializeField] private GameObject[] standaloneSettingObjects;

#if UNITY_STANDALONE
        private int resolutionIndex = 0;
        private Resolution[] resolutions;
        private TMP_Text fullscreenText;
#endif

        void Start()
        {
#if UNITY_STANDALONE
            fullscreenText = fullscreenButton.GetComponentInChildren<TMP_Text>();

            // Set resolution array
            resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
            for (int i = 0; i < resolutions.Length; i++)
            {
                // Check for current resolution
                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    resolutionIndex = i;
            }
#else
            // Deactivate unused settings
            foreach (GameObject settingObject in standaloneSettingObjects)
                settingObject.SetActive(false);
#endif

            // Set initial UI
            versionText.text = $"Version {Application.version}";
            RefreshAllSettings();
        }

        /// <summary>
        /// Start playing the game, either from the save data or from the beginning if no data exists.
        /// </summary>
        public void Play()
        {
            if(SaveLoad.SaveExists())
            {
                sceneTransition.LoadScene("AreaSelect");
            }
            else
            {
                sceneTransition.LoadScene("Cutscene");
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
            mainMixer.GetFloat("musicVolume", out float volume);

            // Modify current volume
            float modifiedVolume = Mathf.Clamp(volume + value, -80, 0);
            mainMixer.SetFloat("musicVolume", modifiedVolume);
            PlayerPrefs.SetFloat("MusicVolume", modifiedVolume);

            // Update UI
            RefreshMusicVolume();
        }

        /// <summary>
        /// Modify the volume of the game's sound effects.
        /// </summary>
        /// <param name="value">The value to increase or decrease the volume by.</param>
        public void ModifyFXVolume(float value)
        {
            mainMixer.GetFloat("fxVolume", out float volume);

            // Modify current volume
            float modifiedVolume = Mathf.Clamp(volume + value, -80, 0);
            mainMixer.SetFloat("fxVolume", modifiedVolume);
            PlayerPrefs.SetFloat("FxVolume", modifiedVolume);

            // Update UI
            RefreshFXVolume();
        }

#if UNITY_STANDALONE
        /// <summary>
        /// Toggle whether the game is in fullscreen or windowed mode.
        /// </summary>
        public void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;

            // Update UI
            RefreshFullscreen();
        }

        /// <summary>
        /// Increment the current resolution index by 1.
        /// </summary>
        public void IncrementResolution()
        {
            // Increment index
            resolutionIndex++;
            if (resolutionIndex >= resolutions.Length)
                resolutionIndex = 0;

            SetResolution();
        }

        /// <summary>
        /// Decrement the current resolution index by 1.
        /// </summary>
        public void DecrementResolution()
        {
            // Decrement index
            resolutionIndex--;
            if (resolutionIndex < 0)
                resolutionIndex = resolutions.Length - 1;

            SetResolution();
        }

        /// <summary>
        /// Set the current screen resolution.
        /// </summary>
        private void SetResolution()
        {
            // Set new resolution
            Resolution resolution = Screen.resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            PlayerPrefs.SetInt("ScreenWidth", resolution.width);
            PlayerPrefs.SetInt("ScreenHeight", resolution.width);

            // Update UI
            RefreshResolution();
        }
#endif

        /// <summary>
        /// Update the UI to show the current settings.
        /// </summary>
        public void RefreshAllSettings()
        {
            RefreshMusicVolume();
            RefreshFXVolume();
#if UNITY_STANDALONE
            RefreshFullscreen();
            RefreshResolution();
#endif
        }

        /// <summary>
        /// Update the UI to show the current music volume.
        /// </summary>
        private void RefreshMusicVolume()
        {
            mainMixer.GetFloat("musicVolume", out float volume);
            musicVolumeLabel.text = string.Format("MUSIC volume: {0}%", (int)((volume + 80) * 1.25f));
        }

        /// <summary>
        /// Update the UI to show the current sound effects volume.
        /// </summary>
        private void RefreshFXVolume()
        {
            mainMixer.GetFloat("fxVolume", out float volume);
            fxVolumeLabel.text = string.Format("EFFECTS volume: {0}%", (int)((volume + 80) * 1.25f));
        }

#if UNITY_STANDALONE
        /// <summary>
        /// Update the UI to show the current screen mode.
        /// </summary>
        private void RefreshFullscreen()
        {
            fullscreenText.text = Screen.fullScreen ? "Set to WINDOWED" : "Set to FULLSCREEN";
        }

        /// <summary>
        /// Update the UI to show the current resolution.
        /// </summary>
        private void RefreshResolution()
        {
            Resolution resolution = resolutions[resolutionIndex];
            resolutionText.text = $"RESOLUTION: {resolution.width} x {resolution.height}";
        }
#endif

#endregion
    }
}