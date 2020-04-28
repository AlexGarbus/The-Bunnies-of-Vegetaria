using System;
using System.Collections;
using System.Collections.Generic;
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
            areaName = areaName.Replace(" ", string.Empty);

            if (Enum.TryParse(areaName, false, out Globals.Area area) && (int)area <= SaveData.current.areasUnlocked)
            {
                gameManager.AreaIndex = (int)area;
                sceneTransition.LoadScene(4);
            }
        }

        public void DisplayBunnyStats(int typeIndex)
        {
            Globals.BunnyType type = (Globals.BunnyType)typeIndex;
            Bunny bunny = null;
            string typeString = "";

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

            string stats = $"{bunny.name} the {typeString}" + "\n\n"
                + $"LEVEL: {bunny.Level}" + spacer + $"EXPERIENCE: {bunny.Experience}" + "\n\n"
                + $"HEALTH: {bunny.maxHealth}" + spacer + $"SKILL: {bunny.maxSkill}" + "\n\n"
                + $"ATTACK: {new string('*', bunny.attack)}" + spacer 
                + $"DEFENSE: {new string('*', bunny.defense)}" + spacer
                + $"SPEED: {new string('*', bunny.speed)}";
            statText.text = stats;
        }
    }
}