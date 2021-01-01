using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class NamingMenu : MonoBehaviour
    {
        [SerializeField] private SceneTransition sceneTransition;

        [Header("Name Input")]
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Image bunnyImage;
        [Tooltip("The sprites to use for each bunny type. These should be ordered according to the BunnyType enum.")]
        [SerializeField] private Sprite[] bunnySprites;
        [SerializeField] private TMP_InputField nameInput;

        [Header("Confirmation Panel")]
        [SerializeField] private GameObject confirmationPanel;
        [SerializeField] private TMP_Text confirmationNames;

        /// <summary>
        /// Struct used for reading descriptions from JSON.
        /// </summary>
        [System.Serializable]
        private struct DescriptionHolder
        {
            public string[] descriptions;
        }

        private int namesInput = 0;
        private DescriptionHolder descriptionHolder;
        private GameManager gameManager;

        private void Start()
        {
            descriptionHolder = JsonUtility.FromJson<DescriptionHolder>(Resources.Load<TextAsset>("Text Assets/naming").text);
            gameManager = GameManager.Instance;

            // Start prompting for names
            PromptNextName();
        }

        /// <summary>
        /// Start the Fade To Next coroutine.
        /// </summary>
        public void StartFadeToNext()
        {
            StartCoroutine(FadeToNext());
        }

        /// <summary>
        /// Fade to the next name input, or the confirmation screen if all names have been input.
        /// </summary>
        private IEnumerator FadeToNext()
        {
            if (nameInput.text.Length == 0)
                yield break;

            // Fade out from previous screen
            StartCoroutine(sceneTransition.FadeOut());
            while (sceneTransition.isFading)
                yield return null;

            // Reset name input if at confirmation screen
            if (confirmationPanel.activeSelf)
            {
                confirmationPanel.SetActive(false);
                namesInput = 0;
            }

            // Set up the next input prompt
            if (namesInput <= 4)
                PromptNextName();

            // Fade in to next screen
            StartCoroutine(sceneTransition.FadeIn());
            while (sceneTransition.isFading)
                yield return null;
        }

        /// <summary>
        /// Prompt the user for the next name input based on the number of names already input.
        /// </summary>
        private void PromptNextName()
        {
            // Save previous name and set next name text
            switch (namesInput)
            {
                case 0:
                    nameInput.text = gameManager.Bunnight.name;
                    break;
                case 1:
                    gameManager.Bunnight.name = nameInput.text;
                    nameInput.text = gameManager.Bunnecromancer.name;
                    break;
                case 2:
                    gameManager.Bunnecromancer.name = nameInput.text;
                    nameInput.text = gameManager.Bunnurse.name;
                    break;
                case 3:
                    gameManager.Bunnurse.name = nameInput.text;
                    nameInput.text = gameManager.Bunneerdowell.name;
                    break;
                case 4:
                    gameManager.Bunneerdowell.name = nameInput.text;
                    ConfirmNames();
                    return;
                default:
                    return;
            }

            // Set next description and image
            descriptionText.text = descriptionHolder.descriptions[namesInput];
            bunnyImage.sprite = bunnySprites[namesInput];

            namesInput++;
        }

        /// <summary>
        /// Prompt the user to confirm that their input names are correct.
        /// </summary>
        private void ConfirmNames()
        {
            confirmationPanel.SetActive(true);
            confirmationNames.text = gameManager.Bunnight.name + "\n\n\n\n" + gameManager.Bunnecromancer.name + "\n\n\n\n" + gameManager.Bunnurse.name + "\n\n\n\n" + gameManager.Bunneerdowell.name;
        }
    }
}