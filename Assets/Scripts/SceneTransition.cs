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

    public bool IsFading = false;
    public float GetFadeTime() => fadeTime;

    private GameObject fadeImageObject;

    private void Awake()
    {
        fadeImageObject = fadeImage.gameObject;
    }

    private void Start()
    {
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
        if (!fadeImageObject.activeSelf)
            fadeImageObject.SetActive(true);
        SetFadeAlpha(1);

        int alphaChanges = 0;
        float waitTime = fadeTime / alphaChangeCount;

        while (alphaChanges < alphaChangeCount)
        {
            SetFadeAlpha(1f - (float)alphaChanges / alphaChangeCount);
            alphaChanges++;
            yield return new WaitForSeconds(waitTime);
        }

        fadeImageObject.SetActive(false);
        IsFading = false;
    }

    /// <summary>
    /// Fade out to a solid color.
    /// </summary>
    public IEnumerator FadeOut()
    {
        if (IsFading)
            yield break;

        IsFading = true;
        if (!fadeImageObject.activeSelf)
            fadeImageObject.SetActive(true);
        SetFadeAlpha(0);

        int alphaChanges = 0;
        float waitTime = fadeTime / alphaChangeCount;

        while (alphaChanges < alphaChangeCount)
        {
            alphaChanges++;
            SetFadeAlpha((float)alphaChanges / alphaChangeCount);
            yield return new WaitForSeconds(waitTime);
        }

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
    private IEnumerator FadeOutToScene(int sceneIndex)
    {
        StartCoroutine(FadeOut());
        while (IsFading)
            yield return null;

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
    private IEnumerator FadeOutQuit()
    {
        StartCoroutine(FadeOut());
        while (IsFading)
            yield return null;

        Application.Quit();
    }

    /// <summary>
    /// Start the Fade Game Object coroutine.
    /// </summary>
    /// <param name="objectToFade"></param>
    public void StartFadeGameObject(GameObject objectToFade)
    {
        StartCoroutine(FadeGameObject(objectToFade));
    }

    /// <summary>
    /// Fade out to a solid color, toggle a game object's active state, and then fade back in.
    /// </summary>
    /// <param name="objToFade">The game object to fade.</param>
    private IEnumerator FadeGameObject(GameObject objectToFade)
    {
        StartCoroutine(FadeOut());
        while (IsFading)
            yield return null;

        objectToFade.SetActive(!objectToFade.activeSelf);

        StartCoroutine(FadeIn());
        while (IsFading)
            yield return null;
    }

    private void SetFadeAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
