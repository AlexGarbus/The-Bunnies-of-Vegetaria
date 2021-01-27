using System;
using TMPro;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class AreaMenu : MonoBehaviour
    {
        [SerializeField] private SceneTransition sceneTransition;

        [Header("Map")]
        [Tooltip("Buttons should be in ascending order by area.")]
        [SerializeField] private AreaButton[] areaButtons;

        [Header("Party")]
        [SerializeField] private TMP_Text statText;
        [SerializeField] private TMP_Text playtimeText;
        [SerializeField] private GameObject[] gameCompleteStars;

        private const string SPACER = "  ";
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;
            SaveData saveData = SaveData.current;

            // Set up map buttons
            for (int i = 0; i < areaButtons.Length; i++)
            {
                if (i + 1 > saveData.areasUnlocked)
                    areaButtons[i].SetLocked();
            }

            // Set up playtime text
            playtimeText.text = string.Format("Playtime: {0:00}:{1:00}:{2:00}", saveData.hours, saveData.minutes, saveData.seconds);

            // Set up game complete stars
            foreach (GameObject gameCompleteStar in gameCompleteStars)
                gameCompleteStar.SetActive(saveData.isGameComplete);
        }

        /// <summary>
        /// Set the selected area without loading it if unlocked.
        /// </summary>
        /// <param name="areaName">The name of the area to load.</param>
        public void SetArea(string areaName)
        {
            // Remove spaces from the area name
            areaName = areaName.Replace(" ", string.Empty);

            // Set area if unlocked
            if (Enum.TryParse(areaName, false, out Globals.Area area) && (int)area <= SaveData.current.areasUnlocked)
            {
                gameManager.BattleArea = area;
            }
        }

        /// <summary>
        /// Load the selected area if it is unlocked.
        /// </summary>
        /// <param name="areaName">The name of the area to load.</param>
        public void LoadArea(string areaName)
        {
            // Remove spaces from the area name
            areaName = areaName.Replace(" ", string.Empty);

            // Load area if unlocked
            if (Enum.TryParse(areaName, false, out Globals.Area area) && (int)area <= SaveData.current.areasUnlocked)
            {
                gameManager.BattleArea = area;
                sceneTransition.SaveAndLoadScene("Battle");
            }
        }

        /// <summary>
        /// Set the next cutscene to play without loading it.
        /// </summary>
        /// <param name="cutsceneName">The cutscene to play.</param>
        public void SetCutscene(string cutsceneName)
        {
            gameManager.cutsceneFileName.Push(cutsceneName);
        }

        /// <summary>
        /// Load the given cutscene.
        /// </summary>
        /// <param name="cutsceneName">The name of the cutscene to load.</param>
        public void LoadCutscene(string cutsceneName)
        {
            gameManager.cutsceneFileName.Push(cutsceneName);
            sceneTransition.SaveAndLoadScene("Cutscene");
        }

        /// <summary>
        /// Display the stats for a selected bunny.
        /// </summary>
        /// <param name="typeIndex">The integer value of the bunny type to display stats for.</param>
        public void DisplayBunnyStats(int typeIndex)
        {
            Globals.BunnyType type = (Globals.BunnyType)typeIndex;
            Bunny bunny = null;
            string typeString = "";

            // Set bunny and type string
            switch (type)
            {
                case Globals.BunnyType.Bunnight:
                    bunny = gameManager.Bunnight;
                    typeString = "BUNNIGHT";
                    break;
                case Globals.BunnyType.Bunnecromancer:
                    bunny = gameManager.Bunnecromancer;
                    typeString = "BUNNECROMANCER";
                    break;
                case Globals.BunnyType.Bunnurse:
                    bunny = gameManager.Bunnurse;
                    typeString = "BUNNURSE";
                    break;
                case Globals.BunnyType.Bunneerdowell:
                    bunny = gameManager.Bunneerdowell;
                    typeString = "BUNNE'ER-DO-WELL";
                    break;
            }

            // Set stat text
            string stats = $"{bunny.name} the {typeString}" + "\n\n"
                + $"LEVEL: {bunny.Level}" + SPACER + $"EXPERIENCE: {bunny.Experience}" + "\n\n"
                + $"HEALTH: {bunny.MaxHealth}" + SPACER + $"SKILL: {bunny.MaxSkillPoints}" + "\n\n"
                + $"ATTACK: {new string('*', bunny.Attack)}" + SPACER 
                + $"DEFENSE: {new string('*', bunny.Defense)}" + SPACER
                + $"SPEED: {new string('*', bunny.Speed)}";
            statText.text = stats;
        }
    }
}