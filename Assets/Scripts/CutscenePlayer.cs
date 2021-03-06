﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class CutscenePlayer : MonoBehaviour
    {
        [Tooltip("The time delay after each character of a text string is typed.")]
        [SerializeField] private float charDelayTime;
        [Tooltip("The time to display a still after text has finished typing.")]
        [SerializeField] private float stillTime;
        [SerializeField] private AudioSource musicPlayer;

        [Header("UI Components")]
        [SerializeField] private Image cutsceneImage;
        [SerializeField] private TMP_Text cutsceneText;
        [SerializeField] private SceneTransition sceneTransition;

        private Cutscene cutscene;

        private void Start()
        {
            GameManager gameManager = GameManager.Instance;

            // Load cutscene from file
            string cutsceneFileName = "cutscene_introduction";
            if (gameManager.cutsceneFileName.TryPop(out string fileName))
                cutsceneFileName = fileName;
            cutscene = JsonUtility.FromJson<Cutscene>(Resources.Load<TextAsset>($"Text Assets/{cutsceneFileName}").text);

            // Start music
            musicPlayer.clip = Resources.Load<AudioClip>($"Music/{cutscene.music}");
            musicPlayer.Play();

            // Start cutscene
            StartCoroutine(Play());

            // Set values for next scene
            if (cutscene.nextArea != Globals.Area.None)
                gameManager.BattleArea = cutscene.nextArea;
            gameManager.startBattleAtBoss.Push(cutscene.startAtBoss);
            gameManager.cutsceneFileName.Push(cutscene.nextCutscene);
        }

        /// <summary>
        /// Play a cutscene, displaying stills with images and text.
        /// </summary>
        private IEnumerator Play()
        {
            WaitForSeconds charDelay = new WaitForSeconds(charDelayTime);
            WaitForSeconds stillDelay = new WaitForSeconds(stillTime);

            for (int i = 0; i < cutscene.text.Length; i++)
            {
                // Fade out from previous still
                if (i > 0)
                    StartCoroutine(sceneTransition.FadeOut());
                while (sceneTransition.isFading)
                    yield return null;

                // Set next still
                if(i < cutscene.spriteFileNames.Length)
                    cutsceneImage.sprite = Resources.Load<Sprite>($"Sprites/Cutscenes/{cutscene.spriteFileNames[i]}");
                cutsceneText.text = "";

                // Fade in to next still
                StartCoroutine(sceneTransition.FadeIn());
                while (sceneTransition.isFading)
                    yield return null;

                // Type out text character-by-character
                for(int j = 0; j <= cutscene.text[i].Length; j++)
                {
                    cutsceneText.text = cutscene.text[i].Insert(j, "<color=#00000000>") + "</color>";
                    yield return GetSkipInput() ? null : charDelay;
                }

                yield return GetSkipInput() ? null : stillDelay;
            }

            sceneTransition.LoadScene(cutscene.sceneToLoad);
        }

        /// <summary>
        /// Check whether any buttons that should skip cutscene playback are held down.
        /// </summary>
        /// <returns>Whether any buttons that should skip cutscene playback are held down.</returns>
        private bool GetSkipInput()
        {
            return Input.GetButton("Submit") || Input.GetButton("Fire1");
        }
    }
}