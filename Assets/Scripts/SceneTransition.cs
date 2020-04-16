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

    public bool IsFading { get; private set; } = false;

    private GameObject fadeImageObject;

    private void Awake()
    {
        fadeImageObject = fadeImage.gameObject;

        if (fadeInOnStart)
            StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
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

    public IEnumerator FadeOut()
    {
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

        fadeImageObject.SetActive(false);
        IsFading = false;
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(FadeOutToScene(sceneIndex));
    }

    public IEnumerator FadeOutToScene(int sceneIndex)
    {
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

    private void SetFadeAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
