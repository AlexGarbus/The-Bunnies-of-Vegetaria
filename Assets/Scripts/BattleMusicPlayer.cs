using System;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class BattleMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip regularBattleMusic;
        [SerializeField] private AudioClip bossBattleMusic;
        [SerializeField] private AudioClip victoryMusic;

        private AudioSource audioSource;
        private bool bossReached = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            BattleHandler.OnEnemiesInitialized += BattleHandler_OnEnemiesInitialized;
            EnemyActor.OnDefeat += EnemyActor_OnDefeat;
        }

        private void Start()
        {
            // TODO: Check for final area.
            PlayMusic(regularBattleMusic);
        }

        private void OnDisable()
        {
            BattleHandler.OnEnemiesInitialized -= BattleHandler_OnEnemiesInitialized;
            EnemyActor.OnDefeat -= EnemyActor_OnDefeat;
        }

        private void BattleHandler_OnEnemiesInitialized(object sender, BattleEventArgs e)
        {
            if (e.isBossWave)
            {
                bossReached = true;
                // TODO: Check for final area.
                PlayMusic(bossBattleMusic);
            }
        }

        private void EnemyActor_OnDefeat(object sender, EventArgs e)
        {
            if (bossReached)
                PlayMusic(victoryMusic, false);
        }

        /// <summary>
        /// Set and play the current music track.
        /// </summary>
        /// <param name="audioClip">The music track to play.</param>
        /// <param name="loop">Whether to loop the music when it finishes playing.</param>
        private void PlayMusic(AudioClip audioClip, bool loop = true)
        {
            audioSource.clip = audioClip;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }
}