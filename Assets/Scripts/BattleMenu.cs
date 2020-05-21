using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
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

        public BunnyActor SelectedBunny { private get; set; }

        private bool ShowSkills => SelectedBunny != null && SelectedBunny.AvailableSkillStrings.Length != 0;

        private Button[] skillButtons;
        private Button[] enemyButtons;

        private void Awake()
        {
            skillButtons = skillPanel.GetComponentsInChildren<Button>();
            enemyButtons = enemyPanel.GetComponentsInChildren<Button>();

            // Hide all menus
            playerStatPanel.SetActive(false);
            playerInputPanel.SetActive(false);
            backButton.SetActive(false);
            optionPanel.SetActive(false);
            skillPanel.SetActive(false);
            enemyPanel.SetActive(false);
            turnPanel.SetActive(false);
        }

        #region Player Stats

        public void ShowPlayerStatPanel(bool isActive)
        {
            playerStatPanel.SetActive(isActive);
        }

        /// <summary>
        /// Set the player stat text to display the current stats of the party.
        /// </summary>
        /// <param name="bunnyActors">The bunny actors to display stats for.</param>
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

        #endregion

        #region Player Input

        public void ShowPlayerInputPanel(bool isActive)
        {
            playerInputPanel.SetActive(isActive);
        }

        /// <summary>
        /// Prompt the player to input their turn.
        /// </summary>
        public void PromptPlayerInput()
        {
            inputPromptText.text = $"What will {SelectedBunny.FighterName} do?";
        }

        public void ShowBackButton(bool isActive)
        {
            backButton.SetActive(isActive);
        }

        /// <summary>
        /// Go back to a previous menu.
        /// </summary>
        public void GoBack()
        {
            optionPanel.SetActive(true);
            PromptPlayerInput();

            if (enemyPanel.activeSelf)
                ShowEnemyPanel(false);
            if (skillPanel.activeSelf)
                ShowSkillPanel(false);
        }

        public void ShowOptionPanel(bool isActive)
        {
            optionPanel.SetActive(isActive);
            skillOptionButton.SetActive(ShowSkills);
        }

        public void ShowSkillPanel(bool isActive)
        {
            skillPanel.SetActive(isActive);
            backButton.SetActive(isActive);
        }

        public void ShowEnemyPanel(bool isActive)
        {
            enemyPanel.SetActive(isActive);
            backButton.SetActive(isActive);
        }

        /// <summary>
        /// Set the number of active enemy buttons and the enemy names on each button.
        /// </summary>
        /// <param name="enemyActors">The enemy actors to set buttons for.</param>
        public void SetEnemyButtons(EnemyActor[] enemyActors)
        {
            for (int i = 0; i < enemyActors.Length && i < enemyButtons.Length; i++)
            {
                Button button = enemyButtons[i];
                EnemyActor actor = enemyActors[i];

                if (actor.IsAlive)
                {
                    // Activate button
                    button.GetComponentInChildren<TMP_Text>().text = actor.FighterName;
                    if (!button.gameObject.activeSelf)
                        button.gameObject.SetActive(true);
                }
                else if (button.gameObject.activeSelf)
                {
                    // Hide button
                    button.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Set the number of active skill buttons and the text on each button.
        /// </summary>
        public void SetSkillButtons()
        {
            if (!ShowSkills)
                return;

            string[] availableSkills = SelectedBunny.AvailableSkillStrings;
            
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

        #endregion

        #region Turns

        public void ShowTurnPanel(bool isActive)
        {
            turnPanel.SetActive(isActive);
        }

        /// <summary>
        /// Display a message to the player describing the current turn.
        /// </summary>
        /// <param name="message">The message that describes the current turn.</param>
        public void SetTurnText(string message)
        {
            turnText.text = message;
        }

        #endregion
    }
}