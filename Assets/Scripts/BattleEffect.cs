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
        /// Play a slash animation and display the damage taken.
        /// </summary>
        /// <param name="damage">The amount of damage taken.</param>
        public void PlaySlash(int damage)
        {
            animator.SetTrigger(animSlash);
            ShowHealthCanvas($"-{damage}");
        }

        /// <summary>
        /// Play a heal animation and display the amount of health healed.
        /// </summary>
        /// <param name="damage">The amount of health healed.</param>
        public void PlayHeal(int healAmount)
        {
            animator.SetTrigger(animHeal);
            ShowHealthCanvas($"+{healAmount}");
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
            Invoke("HideHealthCanvas", healthTime);
        }

        private void HideHealthCanvas() => healthCanvas.enabled = false;
    }
}