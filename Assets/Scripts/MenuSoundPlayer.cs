using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class MenuSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip buttonClickSound;
        [Tooltip("The buttons to play sounds for.")]
        [SerializeField] private Button[] targetButtons;

        private AudioSource audioSource;
        private UnityAction clickAction;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            // Ensure target buttons are not null
            if (targetButtons == null)
                TargetChildButtons();

            // Play sound on click
            clickAction = () => audioSource.PlayOneShot(buttonClickSound);
            foreach(Button button in targetButtons)
                button.onClick.AddListener(clickAction);
        }

        private void OnDestroy()
        {
            // Remove event listeners
            foreach (Button button in targetButtons)
                button.onClick.RemoveListener(clickAction);
        }
        
        /// <summary>
        /// Target all buttons that are on or children of this game object.
        /// </summary>
        [ContextMenu("Target Child Buttons")]
        private void TargetChildButtons()
        {
            targetButtons = GetComponentsInChildren<Button>();
        }
    }
}