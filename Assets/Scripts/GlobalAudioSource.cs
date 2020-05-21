using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class GlobalAudioSource : MonoSingleton<GlobalAudioSource>
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource fxSource;

        public void PlayMusic(AudioClip musicClip)
        {
            if (musicSource == null)
                return;

            musicSource.clip = musicClip;
            musicSource.Play();
        }

        public void PlaySoundEffect(AudioClip soundClip)
        {
            if (fxSource == null)
                return;

            fxSource.clip = soundClip;
            fxSource.Play();
        }

        public void PlaySoundEffectOneShot(AudioClip soundClip)
        {
            if (fxSource == null)
                return;

            fxSource.PlayOneShot(soundClip);
        }
    }
}