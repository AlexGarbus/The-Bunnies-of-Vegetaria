using UnityEngine;
using UnityEngine.Audio;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource fxSource;

        public AudioMixer Mixer => mixer;

        public void PlayMusic(AudioClip musicClip)
        {
            // TODO: Use this instead of individual music source objects in each scene
            musicSource.clip = musicClip;
            musicSource.Play();
        }

        public void PlaySoundEffect(AudioClip soundClip)
        {
            fxSource.PlayOneShot(soundClip);
        }
    }
}