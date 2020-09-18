using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class ButtonSoundController : MonoBehaviour
    {
        private AudioSource audioSource;
        private Button button;
        private UnityAction clickAction;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            button = GetComponent<Button>();

            // FIXME: Can not play a disabled audio source
            // Play sound on click
            clickAction = () => audioSource.Play();
            button.onClick.AddListener(clickAction);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(clickAction);
        }
    }
}