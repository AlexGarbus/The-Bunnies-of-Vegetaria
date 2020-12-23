using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace TheBunniesOfVegetaria
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameObject cursorPrefab;
        [SerializeField] private AudioMixer mainMixer;

        public bool StartBattleAtBoss { get; set; } = false; // TODO: Encapsulate this property better?
        public bool IsCutsceneReady { get; private set; } = false;
        public string CutsceneFile
        {
            get
            {
                IsCutsceneReady = false;
                return cutsceneFile;
            }
            set
            {
                IsCutsceneReady = true;
                cutsceneFile = value;
            }
        }
        public Globals.Area BattleArea { get; set; } = Globals.Area.LettuceFields;
        public Bunny Bunnight { get; private set; }
        public Bunny Bunnecromancer { get; private set; }
        public Bunny Bunnurse { get; private set; }
        public Bunny Bunneerdowell { get; private set; }
        public Bunny[] Party { get; private set; }
        
        private bool isPlaying = false;
        private string cutsceneFile = "cutscene_introduction";

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
            if (isPlaying)
                AddPlaytime();
        }

        private void OnApplicationQuit()
        {
            // Auto-save when the game is quit while playing
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

            // Set whether the game is being played
            isPlaying = scene.name == "AreaSelect" || scene.name == "Battle";
        }

        /// <summary>
        /// Load settings from PlayerPrefs.
        /// </summary>
        private void LoadSettings()
        {
            // Load volume
            mainMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("MusicVolume", 0));
            mainMixer.SetFloat("fxVolume", PlayerPrefs.GetFloat("FxVolume", 0));

            // Load fullscreen mode
            if(PlayerPrefs.HasKey("Fullscreen"))
                Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;

            // Load screen resolution
            if (PlayerPrefs.HasKey("ScreenWidth") && PlayerPrefs.HasKey("ScreenHeight"))
                Screen.SetResolution(PlayerPrefs.GetInt("ScreenWidth"), PlayerPrefs.GetInt("ScreenHeight"), Screen.fullScreen);
        }

        /// <summary>
        /// Create bunny objects from save data.
        /// </summary>
        private void CreateBunnies()
        {
            SaveData save = SaveData.current;
            Party = new Bunny[4];

            // Create bunny objects
            Bunnight = new Bunny(Globals.BunnyType.Bunnight, save.bunnightName, save.bunnightExp);
            Bunnecromancer = new Bunny(Globals.BunnyType.Bunnecromancer, save.bunnecromancerName, save.bunnecromancerExp);
            Bunnurse = new Bunny(Globals.BunnyType.Bunnurse, save.bunnurseName, save.bunnurseExp);
            Bunneerdowell = new Bunny(Globals.BunnyType.Bunneerdowell, save.bunneerdowellName, save.bunneerdowellExp);

            // Store bunnies in array
            Party[(int)Globals.BunnyType.Bunnight] = Bunnight;
            Party[(int)Globals.BunnyType.Bunnecromancer] = Bunnecromancer;
            Party[(int)Globals.BunnyType.Bunnurse] = Bunnurse;
            Party[(int)Globals.BunnyType.Bunneerdowell] = Bunneerdowell;
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