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

        private const string spacer = "  ";
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;

            // Set up map buttons
            for (int i = 0; i < areaButtons.Length; i++)
            {
                if (i + 1 > SaveData.current.areasUnlocked)
                    areaButtons[i].SetLocked();
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
                + $"LEVEL: {bunny.Level}" + spacer + $"EXPERIENCE: {bunny.Experience}" + "\n\n"
                + $"HEALTH: {bunny.MaxHealth}" + spacer + $"SKILL: {bunny.MaxSkillPoints}" + "\n\n"
                + $"ATTACK: {new string('*', bunny.Attack)}" + spacer 
                + $"DEFENSE: {new string('*', bunny.Defense)}" + spacer
                + $"SPEED: {new string('*', bunny.Speed)}";
            statText.text = stats;
        }
    }
}