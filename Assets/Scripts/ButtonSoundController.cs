using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class ButtonSoundController : MonoBehaviour
    {
        private AudioClip clickSound;
        private Button button;
        private GlobalAudioSource audioSource;
        private UnityAction clickAction;

        private void Awake()
        {
            clickSound = Resources.Load<AudioClip>("Sounds/button_click");
            button = GetComponent<Button>();

            // Play sound on click
            clickAction = () => PlayClickSound();
            button.onClick.AddListener(clickAction);
        }

        private void Start()
        {
            audioSource = GlobalAudioSource.Instance;
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(clickAction);
        }

        private void PlayClickSound()
        {
            audioSource.PlaySoundEffectOneShot(clickSound);
        }
    }
}