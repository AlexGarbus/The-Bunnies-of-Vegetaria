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

    // TODO: Make IsFading private for better encapsulation.
    public bool IsFading { get; private set; } = false;
    public float GetFadeTime() => fadeTime;

    private GameObject fadeImageObject;

    private void Awake()
    {
        fadeImageObject = fadeImage.gameObject;

        if (fadeInOnStart)
            StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fade in from a solid color.
    /// </summary>
    public IEnumerator FadeIn()
    {
        if (IsFading)
            yield break;

        IsFading = true;
        fadeImageObject.SetActive(true);
        SetFadeAlpha(1);

        int alphaChanges = 0;
        float waitTime = fadeTime / (alphaChangeCount);

        while (alphaChanges < alphaChangeCount)
        {
            SetFadeAlpha(1f - (float)alphaChanges / alphaChangeCount);
            alphaChanges++;
            yield return new WaitForSeconds(waitTime);
        }

        fadeImageObject.SetActive(false);
        IsFading = false;
    }

    // TODO: Combine the various FadeOut methods for conciseness.
    /// <summary>
    /// Fade out to a solid color.
    /// </summary>
    public IEnumerator FadeOut()
    {
        if (IsFading)
            yield break;

        IsFading = true;
        fadeImageObject.SetActive(true);
        SetFadeAlpha(0);

        int alphaChanges = 0;
        float waitTime = fadeTime / (alphaChangeCount);

        while (alphaChanges < alphaChangeCount)
        {
            alphaChanges++;
            SetFadeAlpha((float)alphaChanges / alphaChangeCount);
            yield return new WaitForSeconds(fadeTime / (alphaChangeCount));
        }

        // FIXME: This causes issues when fading out and in again.
        fadeImageObject.SetActive(false);
        IsFading = false;
    }

    /// <summary>
    /// Start the FadeOutToScene coroutine.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load.</param>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(FadeOutToScene(sceneIndex));
    }

    /// <summary>
    /// Fade out to a solid color, then load a new scene.
    /// </summary>
    /// <param name="sceneIndex">The build index of the scene to load.</param>
    public IEnumerator FadeOutToScene(int sceneIndex)
    {
        if (IsFading)
            yield break;

        IsFading = true;
        fadeImageObject.SetActive(true);
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

    /// <summary>
    /// Start the FadeOutQuit coroutine.
    /// </summary>
    public void Quit()
    {
        StartCoroutine(FadeOutQuit());
    }

    /// <summary>
    /// Fade out to a solid color, then quit the application.
    /// </summary>
    public IEnumerator FadeOutQuit()
    {
        if (IsFading)
            yield break;

        IsFading = true;
        fadeImageObject.SetActive(true);
        SetFadeAlpha(0);

        int alphaChanges = 0;
        float waitTime = fadeTime / (alphaChangeCount);

        while (alphaChanges < alphaChangeCount)
        {
            alphaChanges++;
            SetFadeAlpha((float)alphaChanges / alphaChangeCount);
            yield return new WaitForSeconds(fadeTime / (alphaChangeCount));
        }

        Application.Quit();
    }

    private void SetFadeAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
