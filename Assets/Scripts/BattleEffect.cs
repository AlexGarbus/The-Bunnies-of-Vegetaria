using System.Collections;
using TMPro;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(Animator))]
    public class BattleEffect : MonoBehaviour
    {
        [SerializeField] private float healthTime;
        [SerializeField] private Canvas healthCanvas;
        [SerializeField] private TMP_Text healthText;

        public bool IsPlaying => healthCanvas.enabled;

        private int animSlash, animHeal;
        private Animator animator;
        private Coroutine showHealthCoroutine;

        private void Start()
        {
            // Get animator values
            animator = GetComponent<Animator>();
            animSlash = Animator.StringToHash("Slash");
            animHeal = Animator.StringToHash("Heal");

            // Hide health canvas on start
            healthCanvas.enabled = false;
        }

        /// <summary>
        /// Play a health effect.
        /// </summary>
        /// <param name="deltaHealth">The change in a fighter's health.</param>
        public void PlayHealthEffect(int deltaHealth)
        {
            if (IsPlaying)
                StopCoroutine(showHealthCoroutine);

            if (deltaHealth < 0)
            {
                animator.SetTrigger(animSlash);
                showHealthCoroutine = StartCoroutine(ShowHealthCanvas(deltaHealth.ToString()));
            }
            else
            {
                animator.SetTrigger(animHeal);
                showHealthCoroutine = StartCoroutine(ShowHealthCanvas($"+{deltaHealth}"));
            }
        }

        /// <summary>
        /// Stop and hide the health effect while it is playing.
        /// </summary>
        public void StopHealthEffect()
        {
            if (IsPlaying)
            {
                StopCoroutine(showHealthCoroutine);
                healthCanvas.enabled = false;
            }
        }

        /// <summary>
        /// Display the health canvas for a limited amount of time.
        /// </summary>
        /// <param name="text">The text to display.</param>
        private IEnumerator ShowHealthCanvas(string text)
        {
            // Show canvas
            healthCanvas.enabled = true;
            healthText.text = text;

            // Hide canvas after time has passed
            yield return new WaitForSeconds(healthTime);
            yield return new WaitForEndOfFrame();
            healthCanvas.enabled = false;
        }
    }
}