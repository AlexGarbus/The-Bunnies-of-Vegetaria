using UnityEngine;
using UnityEngine.UI;

namespace TheBunniesOfVegetaria
{
    public class ButtonSoundController : MonoBehaviour
    {
        private AudioClip clickSound;
        private AudioManager audioManager;

        private void Start()
        {
            clickSound = Resources.Load<AudioClip>("Sounds/button_click");
            audioManager = GameManager.Instance.AudioManager;
            GetComponent<Button>().onClick.AddListener(() => PlayClickSound());
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveListener(() => PlayClickSound());
        }

        private void PlayClickSound()
        {
            audioManager.PlaySoundEffect(clickSound);
        }
    }
}