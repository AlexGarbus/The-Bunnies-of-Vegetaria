using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Canvas))]
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private int alphaChangeCount;
        [SerializeField] private float fadeTime;
        [SerializeField] private bool fadeInOnStart = true;

        [HideInInspector] public bool isFading = false;

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
            if (isFading)
                yield break;

            isFading = true;
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
            isFading = false;
        }

        /// <summary>
        /// Fade out to a solid color.
        /// </summary>
        public IEnumerator FadeOut()
        {
            if (isFading)
                yield break;

            isFading = true;
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

            isFading = false;
        }

        /// <summary>
        /// Start the FadeOutToScene coroutine.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void LoadScene(string sceneName)
        {
            StartCoroutine(FadeOutToScene(sceneName));
        }

        /// <summary>
        /// Save the player's data and then start the FadeOutToScene coroutine.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void SaveAndLoadScene(string sceneName)
        {
            SaveLoad.Save();
            StartCoroutine(FadeOutToScene(sceneName));
        }

        /// <summary>
        /// Fade out to a solid color, then load a new scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        private IEnumerator FadeOutToScene(string sceneName)
        {
            StartCoroutine(FadeOut());
            while (isFading)
                yield return null;

            SceneManager.LoadScene(sceneName);
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
            while (isFading)
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
            while (isFading)
                yield return null;

            objectToFade.SetActive(!objectToFade.activeSelf);

            StartCoroutine(FadeIn());
            while (isFading)
                yield return null;
        }

        private void SetFadeAlpha(float alpha)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}