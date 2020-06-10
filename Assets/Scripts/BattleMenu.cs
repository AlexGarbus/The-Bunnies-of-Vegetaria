using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Canvas))]
    public class BattleMenu : MonoBehaviour
    {
        [Header("Player Stats")]
        [SerializeField] private GameObject playerStatPanel;
        [SerializeField] private TMP_Text playerStatText;

        [Header("Player Input")]
        [SerializeField] private GameObject playerInputPanel;
        [SerializeField] private TMP_Text inputPromptText;
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject optionPanel;
        [SerializeField] private GameObject skillOptionButton;
        [SerializeField] private GameObject skillPanel;
        [SerializeField] private GameObject enemyPanel;

        [Header("Turns")]
        [SerializeField] private GameObject turnPanel;
        [SerializeField] private TMP_Text turnText;

        [Header("Messages")]
        [Tooltip("The message that will display when a user is selecting an option. Use {0} in place of a bunny's name.")]
        [SerializeField] private string optionMessage;
        [Tooltip("The message that will display when a user is selecting an enemy to attack. Use {0} in place of a bunny's name.")]
        [SerializeField] private string attackMessage;
        [Tooltip("The message that will display when a user is selecting a skill. Use {0} in place of a bunny's name.")]
        [SerializeField] private string skillMessage;

        private string bunnyName;
        private Button[] skillButtons;
        private Button[] enemyButtons;
        private Canvas canvas;

        private void Awake()
        {
            // Get buttons from panels
            skillButtons = skillPanel.GetComponentsInChildren<Button>();
            enemyButtons = enemyPanel.GetComponentsInChildren<Button>();

            // Disable canvas
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;

            // Prepare panels for player input
            backButton.SetActive(false);
            optionPanel.SetActive(true);
            skillPanel.SetActive(false);
            enemyPanel.SetActive(false);
            turnPanel.SetActive(false);
        }

        public void SetCanvasEnabled(bool isEnabled) => canvas.enabled = isEnabled;

        public void ShowPlayerInputPanel()
        {
            playerInputPanel.SetActive(true);
            turnPanel.SetActive(false);
        }

        public void ShowTurnPanel()
        {
            turnPanel.SetActive(true);
            playerInputPanel.SetActive(false);
        }

        /// <summary>
        /// Set and display the stats of multiple bunnies.
        /// </summary>
        /// <param name="bunnyActors">The bunnies to display stats for.</param>
        public void SetPlayerStatText(BunnyActor[] bunnyActors)
        {
            string stats = string.Empty;

            for(int i = 0; i < bunnyActors.Length; i++)
            {
                // Add a new line of stat text
                BunnyActor bunnyActor = bunnyActors[i];
                stats += string.Format("{0, -16} HP:{1, 3} SP:{2, 3}", bunnyActor.FighterName, bunnyActor.CurrentHealth, bunnyActor.CurrentSkillPoints);
                if (i < bunnyActors.Length - 1)
                    stats += "\n\n";
            }

            playerStatText.text = stats;
        }

        /// <summary>
        /// Set and display the options for a specific bunny.
        /// </summary>
        /// <param name="bunnyActor">The bunny to set options for.</param>
        public void SetOptions(BunnyActor bunnyActor)
        {
            bunnyName = bunnyActor.FighterName;
            DisplayOptionPrompt();

            // Set skills
            if (bunnyActor.AvailableSkillStrings.Length == 0)
            {
                skillOptionButton.SetActive(false);
            }
            else
            {
                skillOptionButton.SetActive(true);
                SetSkillButtons(bunnyActor);
            }
        }

        /// <summary>
        /// Set the number of active enemy buttons and the enemy names on each button.
        /// </summary>
        /// <param name="enemyActors">The enemies to set buttons for.</param>
        public void SetEnemyButtons(EnemyActor[] enemyActors)
        {
            int i;

            // Set a button for each enemy
            for (i = 0; i < enemyActors.Length && i < enemyButtons.Length; i++)
            {
                Button button = enemyButtons[i];
                EnemyActor actor = enemyActors[i];

                // Set text and show button
                button.GetComponentInChildren<TMP_Text>().text = actor.FighterName;
                if (!button.gameObject.activeSelf)
                    button.gameObject.SetActive(true);
            }

            // Hide remaining buttons
            for (int j = i; j < enemyButtons.Length; j++)
            {
                Button button = enemyButtons[j];
                if (button.gameObject.activeSelf)
                    button.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Display a message to the player describing the current turn.
        /// </summary>
        /// <param name="message">The message that describes the current turn.</param>
        public void SetTurnText(string message)
        {
            turnText.text = message;
        }

        /// <summary>
        /// Set the number of active skill buttons and the text on each button.
        /// </summary>
        /// <param name="bunnyActor">The bunny to set skill buttons for.</param>
        private void SetSkillButtons(BunnyActor bunnyActor)
        {
            string[] availableSkills = bunnyActor.AvailableSkillStrings;
            
            for (int i = 0; i < skillButtons.Length; i++)
            {
                Button button = skillButtons[i];

                if (i < availableSkills.Length)
                {
                    // Activate button
                    button.GetComponentInChildren<TMP_Text>().text = availableSkills[i];
                    if (!button.gameObject.activeSelf)
                        skillButtons[i].gameObject.SetActive(true);
                }
                else if (button.gameObject.activeSelf)
                {
                    // Hide button
                    skillButtons[i].gameObject.SetActive(false);
                }
            }
        }

        #region Input Prompt Messages

        public void DisplayOptionPrompt() => inputPromptText.text = string.Format(optionMessage, bunnyName);
        public void DisplayAttackPrompt() => inputPromptText.text = string.Format(attackMessage, bunnyName);
        public void DisplaySkillPrompt() => inputPromptText.text = string.Format(skillMessage, bunnyName);

        #endregion
    }
}