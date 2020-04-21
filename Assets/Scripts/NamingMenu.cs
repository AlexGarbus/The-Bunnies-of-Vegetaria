using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class NamingMenu : MonoBehaviour
{
    [SerializeField] private SceneTransition sceneTransition;

    [Header("Name Input")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image bunnyImage;
    [SerializeField] private Sprite[] bunnySprites;
    [SerializeField] private TMP_InputField nameInput;

    [Header("Confirmation Panel")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TMP_Text confirmationNames;

    // Holder struct is needed to read descriptions with JSONUtility
    [System.Serializable]
    private struct DescriptionHolder
    {
        public string[] descriptions;
    }

    private int namesInput = 0;
    private DescriptionHolder descriptionHolder;

    private void Start()
    {
        descriptionHolder = JsonUtility.FromJson<DescriptionHolder>(Resources.Load<TextAsset>("JSON/naming").text);
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

        StartCoroutine(sceneTransition.FadeOut());
        while (sceneTransition.IsFading)
            yield return null;

        if (confirmationPanel.activeSelf)
            confirmationPanel.SetActive(false);
        PromptNextName();

        StartCoroutine(sceneTransition.FadeIn());
        while (sceneTransition.IsFading)
            yield return null;
    }

    /// <summary>
    /// Prompt the user to input the next bunny's name based on the number of names already input.
    /// </summary>
    private void PromptNextName()
    {
        switch (namesInput)
        {
            case 0:
                nameInput.text = SaveData.current.bunnightName;
                break;
            case 1:
                SaveData.current.bunnightName = nameInput.text;
                nameInput.text = SaveData.current.bunnecromancerName;
                break;
            case 2:
                SaveData.current.bunnecromancerName = nameInput.text;
                nameInput.text = SaveData.current.bunnurseName;
                break;
            case 3:
                SaveData.current.bunnurseName = nameInput.text;
                nameInput.text = SaveData.current.bunneerdowellName;
                break;
            case 4:
                SaveData.current.bunneerdowellName = nameInput.text;

                // All names have been input
                ConfirmNames();
                namesInput = 0;
                return;
        }

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
        SaveData save = SaveData.current;
        confirmationNames.text = save.bunnightName + "\n\n\n\n" + save.bunnecromancerName + "\n\n\n\n" + save.bunnurseName + "\n\n\n\n" + save.bunneerdowellName;
    }
}
