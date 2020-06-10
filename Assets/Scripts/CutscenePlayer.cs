using System.Collections;
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

        [Header("UI Components")]
        [SerializeField] private Image cutsceneImage;
        [SerializeField] private TMP_Text cutsceneText;
        [SerializeField] private SceneTransition sceneTransition;

        private Cutscene cutscene;

        private void Start()
        {
            // TODO: Load from game manager
            cutscene = JsonUtility.FromJson<Cutscene>(Resources.Load<TextAsset>("Text Assets/cutscene_introduction").text);

            // Start music
            GlobalAudioSource.Instance.PlayMusic(Resources.Load<AudioClip>($"Music/{cutscene.music}"));

            // Start cutscene
            StartCoroutine(Play());
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
                    yield return charDelay;
                }

                yield return stillDelay;
            }

            sceneTransition.LoadScene(cutscene.sceneToLoad);
        }
    }
}