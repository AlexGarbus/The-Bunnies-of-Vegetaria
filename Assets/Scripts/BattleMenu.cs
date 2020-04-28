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
        [SerializeField] private GameObject optionPanel;
        [SerializeField] private GameObject enemyPanel;
        [SerializeField] private Button[] enemyButtons;

        [Header("Turns")]
        [SerializeField] private GameObject turnPanel;
        [SerializeField] private TMP_Text turnText;

        private void Awake()
        {
            if (playerStatPanel.activeSelf)
                playerStatPanel.SetActive(false);
            if (playerInputPanel.activeSelf)
                playerInputPanel.SetActive(false);
            if (optionPanel.activeSelf)
                optionPanel.SetActive(false);
            if (enemyPanel.activeSelf)
                enemyPanel.SetActive(false);
            if (turnPanel.activeSelf)
                turnPanel.SetActive(false);
        }

        public void ShowPlayerStatPanel(bool isActive)
        {
            playerStatPanel.SetActive(isActive);
        }

        public void SetPlayerStatText(BunnyActor[] bunnyActors)
        {
            string stats = string.Empty;
            for(int i = 0; i < bunnyActors.Length; i++)
            {
                stats += string.Format("{0, -16} HP:{1, 3} SP:{2, 3}", bunnyActors[i].FighterName, bunnyActors[i].CurrentHealth, bunnyActors[i].CurrentSkill);
                if (i < bunnyActors.Length - 1)
                    stats += "\n\n";
            }
            playerStatText.text = stats;
        }

        public void ShowPlayerInputPanel(bool isActive)
        {
            playerInputPanel.SetActive(isActive);
        }

        public void SetInputPromptText(string message)
        {
            inputPromptText.text = message;
        }

        public void ShowOptionPanel(bool isActive)
        {
            optionPanel.SetActive(isActive);
        }

        public void ShowTurnPanel(bool isActive)
        {
            turnPanel.SetActive(isActive);
        }

        public void SetTurnText(string message)
        {
            turnText.text = message;
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
                if (actor.gameObject.activeSelf && actor.IsAlive)
                {
                    button.GetComponentInChildren<TMP_Text>().text = actor.FighterName;

                    if (!button.gameObject.activeSelf)
                        button.gameObject.SetActive(true);
                }
                else if(button.gameObject.activeSelf)
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
    }
}