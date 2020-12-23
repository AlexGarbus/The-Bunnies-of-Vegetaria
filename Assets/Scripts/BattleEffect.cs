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

        private int animSlash, animHeal;
        private Animator animator;

        private void Start()
        {
            // Get animator values
            animator = GetComponent<Animator>();
            animSlash = Animator.StringToHash("Slash");
            animHeal = Animator.StringToHash("Heal");

            // Hide health canvas on start
            HideHealthCanvas();
        }

        /// <summary>
        /// Play a health effect.
        /// </summary>
        /// <param name="deltaHealth">The change in a fighter's health.</param>
        public void PlayHealthEffect(int deltaHealth)
        {
            if (deltaHealth < 0)
            {
                animator.SetTrigger(animSlash);
                ShowHealthCanvas(deltaHealth.ToString());
            }
            else
            {
                animator.SetTrigger(animHeal);
                ShowHealthCanvas($"+{deltaHealth}");
            }
        }

        /// <summary>
        /// Display the health canvas for a limited amount of time.
        /// </summary>
        /// <param name="text">The text to display.</param>
        private void ShowHealthCanvas(string text)
        {
            healthCanvas.enabled = true;
            healthText.text = text;

            // Hide canvas after time has passed
            CancelInvoke("HideHealthCanvas");
            Invoke("HideHealthCanvas", healthTime + 0.01f); // TODO: Use coroutine instead.
        }

        private void HideHealthCanvas() => healthCanvas.enabled = false;
    }
}