using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace TheBunniesOfVegetaria
{ 
    public class Credits : MonoBehaviour
    {
        [SerializeField] private float creditDelay;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private TMP_Text creditsText;

        string[] lines;

        private void Start()
        {
            lines = Resources.Load<TextAsset>("Text Assets/Credits").text.Split(new[] { Environment.NewLine }, StringSplitOptions.None );
            StartCoroutine(PlayCredits());
        }

        private IEnumerator PlayCredits()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    StartCoroutine(sceneTransition.FadeOut());

                while (sceneTransition.isFading)
                    yield return null;

                string[] currentLine = lines[i].Split(',');
                creditsText.text = string.Format("{0}\n\n{1}", currentLine[0], currentLine[1]);

                StartCoroutine(sceneTransition.FadeIn());

                while (sceneTransition.isFading)
                    yield return null;

                yield return new WaitForSeconds(creditDelay);
            }

            sceneTransition.LoadScene("MainMenu");
        }
    }
}