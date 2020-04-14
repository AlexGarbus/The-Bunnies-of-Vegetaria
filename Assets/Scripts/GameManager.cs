using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        // Set singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        SaveLoad.Load();

        // Load settings
        if (PlayerPrefs.HasKey("MusicVolume"))
            mixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        if (PlayerPrefs.HasKey("FxVolume"))
            mixer.SetFloat("fxVolume", PlayerPrefs.GetFloat("FxVolume"));
        if (PlayerPrefs.HasKey("Quality"))
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
        if (PlayerPrefs.HasKey("Fullscreen"))
            Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        if (PlayerPrefs.HasKey("Resolution"))
        {
            Resolution resolution = Screen.resolutions[PlayerPrefs.GetInt("Resolution")];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
    
    void Update()
    {
        Playtime();
    }

    // Update the playtime in the save data
    private void Playtime()
    {
        SaveData save = SaveData.current;
        save.seconds += Time.deltaTime;
        if (save.seconds >= 60)
        {
            save.seconds -= 60;
            save.minutes++;
        }
        if (save.minutes >= 60)
        {
            save.minutes -= 60;
            save.hours++;
        }
        SaveData.current = save;
    }

    // Save data if the game closes
    private void OnApplicationQuit()
    {
        SaveLoad.Save();
    }
}
