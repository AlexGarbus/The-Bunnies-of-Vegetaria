using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace TheBunniesOfVegetaria
{ 
    public class CreditsPlayer : MonoBehaviour
    {
        [SerializeField] private float creditDisplayTime;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private TMP_Text creditsText;

        private const string SEPARATOR = ",";
        private const string SPACER = "\n\n";
        private string[] lines;

        private void Start()
        {
            // Split credits into lines
            lines = Resources.Load<TextAsset>("Text Assets/Credits").text.Split(new[] { Environment.NewLine }, StringSplitOptions.None );

            // Start playing credits
            StartCoroutine(PlayCredits());
        }

        private IEnumerator PlayCredits()
        {
            WaitForSeconds creditDelay = new WaitForSeconds(creditDisplayTime);

            for (int i = 0; i < lines.Length; i++)
            {
                // Fade out from previous credit
                if (i > 0)
                    StartCoroutine(sceneTransition.FadeOut());
                while (sceneTransition.isFading)
                    yield return null;

                // Set next credit
                creditsText.text = lines[i].Replace(SEPARATOR, SPACER);

                // Fade in to next credit
                StartCoroutine(sceneTransition.FadeIn());
                while (sceneTransition.isFading)
                    yield return null;

                yield return creditDelay;
            }

            sceneTransition.LoadScene("MainMenu");
        }
    }
}