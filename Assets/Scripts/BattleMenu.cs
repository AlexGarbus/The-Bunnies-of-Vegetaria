﻿using System.Collections;
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
        [SerializeField] private GameObject skillOptionButton;
        [SerializeField] private GameObject skillPanel;
        [SerializeField] private GameObject enemyPanel;

        [Header("Turns")]
        [SerializeField] private GameObject turnPanel;
        [SerializeField] private TMP_Text turnText;

        private Button[] skillButtons;
        private Button[] enemyButtons;

        private void Awake()
        {
            skillButtons = skillPanel.GetComponentsInChildren<Button>();
            enemyButtons = enemyPanel.GetComponentsInChildren<Button>();

            if (playerStatPanel.activeSelf)
                playerStatPanel.SetActive(false);
            if (playerInputPanel.activeSelf)
                playerInputPanel.SetActive(false);
            if (optionPanel.activeSelf)
                optionPanel.SetActive(false);
            if (skillPanel.activeSelf)
                skillPanel.SetActive(false);
            if (enemyPanel.activeSelf)
                enemyPanel.SetActive(false);
            if (turnPanel.activeSelf)
                turnPanel.SetActive(false);
        }

        #region Player Stats

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

        #endregion

        #region Player Input

        public void ShowPlayerInputPanel(bool isActive, BunnyActor bunnyActor = null)
        {
            playerInputPanel.SetActive(isActive);
            if(bunnyActor)
                skillOptionButton.SetActive(bunnyActor.AvailableSkillStrings.Length != 0);
        }

        public void SetInputPromptText(string message)
        {
            inputPromptText.text = message;
        }

        public void ShowOptionPanel(bool isActive)
        {
            optionPanel.SetActive(isActive);
        }

        public void ShowSkillPanel(bool isActive)
        {
            skillPanel.SetActive(isActive);
        }

        public void ShowEnemyPanel(bool isActive)
        {
            enemyPanel.SetActive(isActive);
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

        /// <summary>
        /// Set the number of active skill buttons and the text on each button.
        /// </summary>
        /// <param name="bunnyActor">The bunny actor to set the buttons for.</param>
        public void SetSkillButtons(BunnyActor bunnyActor)
        {
            string[] availableSkills = bunnyActor.AvailableSkillStrings;

            for (int i = 0; i < skillButtons.Length; i++)
            {
                if (i < availableSkills.Length)
                {
                    skillButtons[i].gameObject.SetActive(true);
                    skillButtons[i].GetComponentInChildren<TMP_Text>().text = availableSkills[i];
                }
                else
                {
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

        public void SetTurnText(string message)
        {
            turnText.text = message;
        }

        #endregion
    }
}