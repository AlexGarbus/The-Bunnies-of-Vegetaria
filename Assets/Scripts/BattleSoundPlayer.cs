using System;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [RequireComponent(typeof(AudioSource))]
    public class BattleSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip attackSound;
        [SerializeField] private AudioClip healSound;
        [SerializeField] private AudioClip bunnyDefeatSound;
        [SerializeField] private AudioClip enemyDefeatSound;
        [SerializeField] private AudioClip levelUpSound;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            BattleHandler.OnBunniesInitialized += BattleHandler_OnBunniesInitialized;
            BattleHandler.OnEnemiesInitialized += BattleHandler_OnEnemiesInitialized;
            BattleHandler.OnWaveWon += BattleHandler_OnWaveWon;
            BattleHandler.OnWaveLost += BattleHandler_OnWaveLost;
            BunnyActor.OnDefeat += BunnyActor_OnDefeat;
            EnemyActor.OnDefeat += EnemyActor_OnDefeat;
        }

        private void OnDisable()
        {
            BattleHandler.OnBunniesInitialized -= BattleHandler_OnBunniesInitialized;
            BattleHandler.OnEnemiesInitialized -= BattleHandler_OnEnemiesInitialized;
            BattleHandler.OnWaveWon -= BattleHandler_OnWaveWon;
            BattleHandler.OnWaveLost -= BattleHandler_OnWaveLost;
            BunnyActor.OnDefeat -= BunnyActor_OnDefeat;
            EnemyActor.OnDefeat -= EnemyActor_OnDefeat;
        }

        /// <summary>
        /// Play the given sound only if the audio source is not playing.
        /// </summary>
        /// <param name="clip">The sound to attempt to play.</param>
        private void PlayBattleSound(AudioClip clip)
        {
            if (!audioSource.isPlaying || audioSource.time > 0)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        #region Event Handlers

        private void BattleHandler_OnBunniesInitialized(object sender, BattleEventArgs e)
        {
            foreach (Bunny bunny in e.bunnies)
                SubscribeToBunnyEvents(bunny);
        }

        private void BattleHandler_OnEnemiesInitialized(object sender, BattleEventArgs e)
        {
            foreach (Enemy enemy in e.enemies)
                SubscribeToEnemyEvents(enemy);
        }

        private void BattleHandler_OnWaveWon(object sender, BattleEventArgs e)
        {
            if (e.isBossWave)
            {
                // FIXME: Need to unsuscribe when exiting to cutscene. Possibly implement OnBattleEnd event in BattleHandler.
                foreach (Bunny bunny in e.bunnies)
                    UnsubscribeFromBunnyEvents(bunny);
            }
        }

        private void BattleHandler_OnWaveLost(object sender, BattleEventArgs e)
        {
            foreach (Bunny bunny in e.bunnies)
                UnsubscribeFromBunnyEvents(bunny);
        }

        private void BunnyActor_OnDefeat(object sender, EventArgs e) => PlayBattleSound(bunnyDefeatSound);

        private void EnemyActor_OnDefeat(object sender, EventArgs e) => PlayBattleSound(enemyDefeatSound);

        private void Bunny_OnDoDamage(object sender, EventArgs e) => PlayBattleSound(attackSound);

        private void Bunny_OnFullRestore(object sender, EventArgs e) => PlayBattleSound(levelUpSound);

        private void Bunny_OnHealthChange(object sender, PointEventArgs e)
        {
            if (e.DeltaPoints >= 0)
                PlayBattleSound(healSound);
        }

        private void Bunny_OnSkillPointsChange(object sender, PointEventArgs e)
        {
            if (e.DeltaPoints >= 0)
                PlayBattleSound(healSound);
        }

        private void Enemy_OnDefeat(object sender, EventArgs e)
        {
            if (sender is Enemy)
            {
                Enemy enemy = sender as Enemy;
                UnsubscribeFromEnemyEvents(enemy);
            }
        }

        private void Enemy_OnDoDamage(object sender, EventArgs e) => PlayBattleSound(attackSound);

        private void Enemy_OnHealthChange(object sender, PointEventArgs e)
        {
            if (e.DeltaPoints >= 0)
                PlayBattleSound(healSound);
        }

        #endregion

        #region Event Subscribing

        /// <summary>
        /// Subscribe to a bunny's events.
        /// </summary>
        private void SubscribeToBunnyEvents(Bunny bunny)
        {
            bunny.OnDoDamage += Bunny_OnDoDamage;
            bunny.OnFullRestore += Bunny_OnFullRestore;
            bunny.OnHealthChange += Bunny_OnHealthChange;
            bunny.OnSkillPointsChange += Bunny_OnSkillPointsChange;
        }

        /// <summary>
        /// Unsubscribe from a bunny's events.
        /// </summary>
        private void UnsubscribeFromBunnyEvents(Bunny bunny)
        {
            bunny.OnDoDamage -= Bunny_OnDoDamage;
            bunny.OnFullRestore -= Bunny_OnFullRestore;
            bunny.OnHealthChange -= Bunny_OnHealthChange;
            bunny.OnSkillPointsChange -= Bunny_OnSkillPointsChange;
        }

        /// <summary>
        /// Subscribe to an enemy's events.
        /// </summary>
        private void SubscribeToEnemyEvents(Enemy enemy)
        {
            enemy.OnDefeat += Enemy_OnDefeat;
            enemy.OnDoDamage += Enemy_OnDoDamage;
            enemy.OnHealthChange += Enemy_OnHealthChange;
        }

        /// <summary>
        /// Unsubscribe from an enemy's events.
        /// </summary>
        private void UnsubscribeFromEnemyEvents(Enemy enemy)
        {
            enemy.OnDefeat -= Enemy_OnDefeat;
            enemy.OnDoDamage -= Enemy_OnDoDamage;
            enemy.OnHealthChange -= Enemy_OnHealthChange;
        }

        #endregion
    }
}