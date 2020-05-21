using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace TheBunniesOfVegetaria
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameObject cursorPrefab;
        [SerializeField] private AudioMixer mainMixer;

        public Globals.Area BattleArea { get; set; } = Globals.Area.LettuceFields;
        public Bunny Bunnight { get; private set; }
        public Bunny Bunnecromancer { get; private set; }
        public Bunny Bunnurse { get; private set; }
        public Bunny Bunneerdowell { get; private set; }
        
        private bool isPlaying = false;

        protected override void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            SaveLoad.Load();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            LoadSettings();
            CreateBunnies();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    
        private void Update()
        {
            if(isPlaying)
                AddPlaytime();
        }

        private void OnApplicationQuit()
        {
            if (isPlaying)
                SaveLoad.Save();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
#if (UNITY_STANDALONE || UNITY_EDITOR)
            // Instantiate custom cursor object
            GameObject cursorObject = Instantiate(cursorPrefab);
            cursorObject.name = "Cursor";
#endif

            // Set whether the game is actually being played
            isPlaying = scene.name == "AreaSelect" || scene.name == "Battle";
        }

        /// <summary>
        /// Load settings from PlayerPrefs.
        /// </summary>
        private void LoadSettings()
        {
            mainMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("MusicVolume", 0));
            mainMixer.SetFloat("fxVolume", PlayerPrefs.GetFloat("FxVolume", 0));
            if(PlayerPrefs.HasKey("Fullscreen"))
                Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
            if (PlayerPrefs.HasKey("ScreenWidth") && PlayerPrefs.HasKey("ScreenHeight"))
                Screen.SetResolution(PlayerPrefs.GetInt("ScreenWidth"), PlayerPrefs.GetInt("ScreenHeight"), Screen.fullScreen);
        }

        /// <summary>
        /// Create bunny objects using save data.
        /// </summary>
        private void CreateBunnies()
        {
            SaveData save = SaveData.current;
            Bunnight = new Bunny(Globals.BunnyType.Bunnight, save.bunnightName, save.bunnightExp);
            Bunnecromancer = new Bunny(Globals.BunnyType.Bunnecromancer, save.bunnecromancerName, save.bunnecromancerExp);
            Bunnurse = new Bunny(Globals.BunnyType.Bunnurse, save.bunnurseName, save.bunnurseExp);
            Bunneerdowell = new Bunny(Globals.BunnyType.Bunneerdowell, save.bunneerdowellName, save.bunneerdowellExp);
        }

        /// <summary>
        /// Add to the total play time in the save data.
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
}