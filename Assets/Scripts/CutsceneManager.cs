using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private TextAsset cutsceneJson;
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
        cutscene = JsonUtility.FromJson<Cutscene>(cutsceneJson.text);
        musicPlayer = GetComponent<AudioSource>();
        musicPlayer.clip = Resources.Load<AudioClip>($"Music/{cutscene.music}");
        musicPlayer.Play();
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        int textIndex = 0;

        while(textIndex < cutscene.text.Length)
        {
            if (textIndex > 0)
                StartCoroutine(sceneTransition.FadeOut());

            while (sceneTransition.IsFading)
                yield return null;

            cutsceneText.text = "";

            StartCoroutine(sceneTransition.FadeIn());

            while (sceneTransition.IsFading)
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
