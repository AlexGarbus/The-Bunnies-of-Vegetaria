using System.Collections;
using TMPro;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class CutsceneManager : MonoBehaviour
    {
        [Tooltip("The time delay after each character of a text string is typed.")]
        [SerializeField] private float charDelay;
        [Tooltip("The time delay after a text string is fully typed.")]
        [SerializeField] private float textDelay;

        [Header("UI Components")]
        [SerializeField] private TMP_Text cutsceneText;
        [SerializeField] private SceneTransition sceneTransition;

        private AudioSource musicPlayer;
        private Cutscene cutscene;

        private void Start()
        {
            cutscene = JsonUtility.FromJson<Cutscene>(Resources.Load<TextAsset>("Text Assets/introduction").text);

            musicPlayer = GetComponent<AudioSource>();
            musicPlayer.clip = Resources.Load<AudioClip>($"Music/{cutscene.music}");
            musicPlayer.Play();

            StartCoroutine(TypeText());
        }

        /// <summary>
        /// Type out each line of cutscene text character-by-character.
        /// </summary>
        private IEnumerator TypeText()
        {
            int textIndex = 0;

            while(textIndex < cutscene.text.Length)
            {
                if (textIndex > 0)
                    StartCoroutine(sceneTransition.FadeOut());

                while (sceneTransition.isFading)
                    yield return null;

                cutsceneText.text = "";

                StartCoroutine(sceneTransition.FadeIn());

                while (sceneTransition.isFading)
                    yield return null;

                for(int i = 0; i < cutscene.text[textIndex].Length; i++)
                {
                    cutsceneText.text += cutscene.text[textIndex][i];
                    yield return new WaitForSeconds(charDelay);
                }

                yield return new WaitForSeconds(textDelay);

                textIndex++;
            }

            sceneTransition.LoadScene(cutscene.sceneToLoad);
        }
    }
}