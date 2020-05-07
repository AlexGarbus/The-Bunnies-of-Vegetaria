using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace TheBunniesOfVegetaria
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private GameObject cursorPrefab;

        public Globals.Area BattleArea { get; set; } = Globals.Area.LettuceFields;
        public AudioManager AudioManager { get; private set; }
        public Bunny Bunnight { get; private set; }
        public Bunny Bunnecromancer { get; private set; }
        public Bunny Bunnurse { get; private set; }
        public Bunny Bunneerdowell { get; private set; }

        private void Awake()
        {
            // Set singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Continue Awake functionality
                SaveLoad.Load();
                AudioManager = GetComponentInChildren<AudioManager>();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
#if UNITY_STANDALONE
            // Instantiate custom cursor object
            GameObject cursorObject = Instantiate(cursorPrefab);
            cursorObject.name = "Cursor";
#endif
        }

        private void Start()
        {
            LoadSettings();
            CreateBunnies();
        }
    
        private void Update()
        {
            if(SceneManager.GetActiveScene().buildIndex > 1)
                AddPlaytime();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnApplicationQuit()
        {
            if (SceneManager.GetActiveScene().buildIndex > 1)
                SaveLoad.Save();
        }

        /// <summary>
        /// Load settings from PlayerPrefs.
        /// </summary>
        private void LoadSettings()
        {
            AudioManager.Mixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("MusicVolume", 0));
            AudioManager.Mixer.SetFloat("fxVolume", PlayerPrefs.GetFloat("FxVolume", 0));
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