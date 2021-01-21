using System;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class BattleMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip regularBattleMusic;
        [SerializeField] private AudioClip finalAreaBattleMusic;
        [SerializeField] private AudioClip bossBattleMusic;
        [SerializeField] private AudioClip finalBossBattleMusic;
        [SerializeField] private AudioClip victoryMusic;

        private bool bossReached = false;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            BattleHandler.OnEnemiesInitialized += BattleHandler_OnEnemiesInitialized;
            EnemyActor.OnDefeat += EnemyActor_OnDefeat;
        }

        private void OnDisable()
        {
            BattleHandler.OnEnemiesInitialized -= BattleHandler_OnEnemiesInitialized;
            EnemyActor.OnDefeat -= EnemyActor_OnDefeat;
        }

        private void BattleHandler_OnEnemiesInitialized(object sender, BattleEventArgs e)
        {
            if (e.isFinalWave)
            {
                // Start boss battle music
                bossReached = true;
                bool isFinalBattle = GameManager.Instance.BattleArea == Globals.Area.Final2;
                PlayMusic(isFinalBattle ? finalBossBattleMusic : bossBattleMusic);
            }
            else if (!audioSource.isPlaying)
            {
                // Start regular enemy battle music
                bool isFinalArea = GameManager.Instance.BattleArea == Globals.Area.CarrotTop;
                PlayMusic(isFinalArea ? finalAreaBattleMusic : regularBattleMusic);
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