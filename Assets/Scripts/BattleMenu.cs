using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Canvas))]
    public class BattleMenu : MonoBehaviour
    {
        [Header("Player Stats")]
        [SerializeField] private Canvas playerStatPanel;
        [SerializeField] private TMP_Text playerStatText;

        [Header("Player Input")]
        [SerializeField] private Canvas playerInputPanel;
        [SerializeField] private TMP_Text inputPromptText;
        [SerializeField] private Canvas backPanel;
        [SerializeField] private Canvas optionPanel;
        [SerializeField] private GameObject skillOptionButton;
        [SerializeField] private Canvas skillPanel;
        [SerializeField] private Canvas enemyPanel;

        [Header("Turns")]
        [SerializeField] private Canvas turnPanel;
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

            // Disable main canvas
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;

            // Prepare sub-canvases for player input
            backPanel.enabled = false;
            optionPanel.enabled = true;
            skillPanel.enabled = false;
            enemyPanel.enabled = false;
            turnPanel.enabled = false;
        }

        private void OnEnable()
        {
            BattleHandler.OnBunniesInitialized += BattleHandler_OnBunniesInitialized;
            BattleHandler.OnSettingUpInput += BattleHandler_OnSettingUpInput;
            BattleHandler.OnTurnPerformed += BattleHandler_OnTurnPerformed;
            BattleHandler.OnWaveWon += BattleHandler_OnWaveWon;
            BattleHandler.OnWaveLost += BattleHandler_OnWaveLost;
            BattleHandler.OnSelectedBunnyChanged += BattleHandler_OnSelectedBunnyChanged;
        }

        private void OnDisable()
        {
            BattleHandler.OnBunniesInitialized -= BattleHandler_OnBunniesInitialized;
            BattleHandler.OnSettingUpInput -= BattleHandler_OnSettingUpInput;
            BattleHandler.OnTurnPerformed -= BattleHandler_OnTurnPerformed;
            BattleHandler.OnWaveWon -= BattleHandler_OnWaveWon;
            BattleHandler.OnWaveLost -= BattleHandler_OnWaveLost;
            BattleHandler.OnSelectedBunnyChanged -= BattleHandler_OnSelectedBunnyChanged;
        }

        #region Event Handlers

        private void BattleHandler_OnBunniesInitialized(object sender, BattleEventArgs e)
        {
            SetPlayerStatText(e.Bunnies);
        }

        private void BattleHandler_OnSettingUpInput(object sender, BattleEventArgs e)
        {
            SetCanvasEnabled(true);
            ShowPlayerInputPanel();
            SetEnemyButtons(e.Enemies);
        }

        private void BattleHandler_OnTurnPerformed(object sender, BattleEventArgs e)
        {
            SetTurnText(e.CurrentTurn.Message);
            SetPlayerStatText(e.Bunnies);
        }

        private void BattleHandler_OnWaveWon(object sender, BattleEventArgs e)
        {
            SetCanvasEnabled(false);
            SetPlayerStatText(e.Bunnies);
        }

        private void BattleHandler_OnWaveLost(object sender, BattleEventArgs e)
        {
            SetCanvasEnabled(false);
        }

        private void BattleHandler_OnSelectedBunnyChanged(object sender, BattleEventArgs e)
        {
            if (e.SelectedBunny == null)
                ShowTurnPanel();
            else
                SetOptions(e.SelectedBunny);
        }

        #endregion

        public void SetCanvasEnabled(bool isEnabled) => canvas.enabled = isEnabled;

        public void ShowPlayerInputPanel()
        {
            playerInputPanel.enabled = true;
            turnPanel.enabled = false;
        }

        public void ShowTurnPanel()
        {
            // FIXME: Player input panel still selectable for a single frame; causes issues
            turnPanel.enabled = true;
            playerInputPanel.enabled = false;
        }

        /// <summary>
        /// Set and display the stats of multiple bunnies.
        /// </summary>
        /// <param name="bunnies">The bunnies to display stats for.</param>
        public void SetPlayerStatText(Bunny[] bunnies)
        {
            string stats = string.Empty;

            for(int i = 0; i < bunnies.Length; i++)
            {
                // Add a new line of stat text
                Bunny bunny = bunnies[i];
                stats += string.Format("{0, -16} HP:{1, 3} SP:{2, 3}", bunny.name, bunny.CurrentHealth, bunny.CurrentSkillPoints);
                if (i < bunnies.Length - 1)
                    stats += "\n\n";
            }

            playerStatText.text = stats;
        }

        /// <summary>
        /// Set and display the options for a specific bunny.
        /// </summary>
        /// <param name="bunny">The bunny to set options for.</param>
        public void SetOptions(Bunny bunny)
        {
            bunnyName = bunny.name;
            DisplayOptionPrompt();

            // Set skills
            if (bunny.GetAvailableSkillStrings().Length == 0)
            {
                skillOptionButton.SetActive(false);
            }
            else
            {
                skillOptionButton.SetActive(true);
                SetSkillButtons(bunny);
            }
        }

        /// <summary>
        /// Set the number of active enemy buttons and the enemy names on each button.
        /// </summary>
        /// <param name="enemies">The enemies to set buttons for.</param>
        public void SetEnemyButtons(Enemy[] enemies)
        {
            for (int i = 0; i < enemyButtons.Length; i++)
            {
                Button button = enemyButtons[i];

                if (i < enemies.Length && enemies[i].IsAlive)
                {
                    // Display button for active enemy
                    button.GetComponentInChildren<TMP_Text>().text = enemies[i].name;
                    if (!button.gameObject.activeSelf)
                        button.gameObject.SetActive(true);
                }
                else if (button.gameObject.activeSelf)
                {
                    // Hide button for inactive enemy
                    button.gameObject.SetActive(false);
                }
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
        /// <param name="bunny">The bunny to set skill buttons for.</param>
        private void SetSkillButtons(Bunny bunny)
        {
            string[] availableSkills = bunny.GetAvailableSkillStrings();
            
            for (int i = 0; i < skillButtons.Length; i++)
            {
                Button button = skillButtons[i];

                if (i < availableSkills.Length)
                {
                    // Activate button
                    button.GetComponentInChildren<TMP_Text>().text = availableSkills[i];
                    if (!button.gameObject.activeSelf)
                        skillButtons[i].gameObject.SetActive(true);
                    button.interactable = bunny.CanUseSkill(i);
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