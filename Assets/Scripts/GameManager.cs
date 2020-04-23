using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private AudioMixer mixer;

    public int AreaIndex { get; set; } = 0;
    public Bunny Bunnight { get; private set; }
    public Bunny Bunnecromancer { get; private set; }
    public Bunny Bunnurse { get; private set; }
    public Bunny Bunneerdowell { get; private set; }

    private void Awake()
    {
        // Set singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SaveLoad.Load();
    }
    
    void Start()
    {
        LoadSettings();
        CreateBunnies();
    }
    
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex > 0)
            AddPlaytime();
    }

    /// <summary>
    /// Load settings from PlayerPrefs
    /// </summary>
    private void LoadSettings()
    {
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

    /// <summary>
    /// Create bunny objects using save data.
    /// </summary>
    private void CreateBunnies()
    {
        SaveData save = SaveData.current;
        Bunnight = new Bunny(BunnyType.Bunnight, save.bunnightName, save.bunnightExp);
        Bunnecromancer = new Bunny(BunnyType.Bunnecromancer, save.bunnecromancerName, save.bunnecromancerExp);
        Bunnurse = new Bunny(BunnyType.Bunnurse, save.bunnurseName, save.bunnurseExp);
        Bunneerdowell = new Bunny(BunnyType.Bunneerdowell, save.bunneerdowellName, save.bunneerdowellExp);
    }

    /// <summary>
    /// Add to the play time in the save data.
    /// </summary>
    private void AddPlaytime()
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
}
