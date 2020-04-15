using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private int alphaChangeCount;
    [SerializeField] private float fadeTime;
    [SerializeField] private bool fadeInOnStart = true;

    private void Start()
    {
        if (fadeInOnStart)
            StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fade the screen to black and then load a scene.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load.</param>
    public void LoadScene(int sceneIndex)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        StartCoroutine(FadeOut(sceneIndex));
    }

    private IEnumerator FadeIn()
    {
        SetFadeAlpha(1);

        int alphaChanges = 0;
        float waitTime = fadeTime / (alphaChangeCount);

        while (alphaChanges < alphaChangeCount)
        {
            SetFadeAlpha(1f - (float)alphaChanges / alphaChangeCount);
            alphaChanges++;
            yield return new WaitForSeconds(waitTime);
        }

        gameObject.SetActive(false);
    }

    private IEnumerator FadeOut(int sceneIndex)
    {
        SetFadeAlpha(0);

        int alphaChanges = 0;
        float waitTime = fadeTime / (alphaChangeCount);

        while (alphaChanges < alphaChangeCount)
        {
            alphaChanges++;
            SetFadeAlpha((float)alphaChanges / alphaChangeCount);
            yield return new WaitForSeconds(fadeTime / (alphaChangeCount));
        }

        SceneManager.LoadScene(sceneIndex);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
