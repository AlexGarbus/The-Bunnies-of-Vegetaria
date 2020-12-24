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

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            BattleHandler.OnBunniesInitialized += BattleHandler_OnBunniesInitialized;
            BattleHandler.OnEnemiesInitialized += BattleHandler_OnEnemiesInitialized;
            BattleHandler.OnBattleEnd += BattleHandler_OnBattleEnd;
            BunnyActor.OnDefeat += BunnyActor_OnDefeat;
            EnemyActor.OnDefeat += EnemyActor_OnDefeat;
        }

        private void OnDisable()
        {
            BattleHandler.OnBunniesInitialized -= BattleHandler_OnBunniesInitialized;
            BattleHandler.OnEnemiesInitialized -= BattleHandler_OnEnemiesInitialized;
            BattleHandler.OnBattleEnd -= BattleHandler_OnBattleEnd;
            BunnyActor.OnDefeat -= BunnyActor_OnDefeat;
            EnemyActor.OnDefeat -= EnemyActor_OnDefeat;
        }

        private void BattleHandler_OnBunniesInitialized(object sender, BattleEventArgs e)
        {
            foreach (Bunny bunny in e.Bunnies)
            {
                bunny.OnDoDamage += Bunny_OnDoDamage;
                bunny.OnHealthChange += Bunny_OnHealthChange;
                bunny.OnSkillPointsChange += Bunny_OnSkillPointsChange;
            }
        }

        private void BattleHandler_OnEnemiesInitialized(object sender, BattleEventArgs e)
        {
            foreach (Enemy enemy in e.Enemies)
            {
                enemy.OnDefeat += Enemy_OnDefeat;
                enemy.OnDoDamage += Enemy_OnDoDamage;
                enemy.OnHealthChange += Enemy_OnHealthChange;
            }
        }

        private void BattleHandler_OnBattleEnd(object sender, BattleEventArgs e)
        {
            foreach (Bunny bunny in e.Bunnies)
            {
                bunny.OnDoDamage -= Bunny_OnDoDamage;
                bunny.OnHealthChange -= Bunny_OnHealthChange;
                bunny.OnSkillPointsChange -= Bunny_OnSkillPointsChange;
            }
        }

        private void BunnyActor_OnDefeat(object sender, EventArgs e) => audioSource.PlayOneShot(bunnyDefeatSound);

        private void EnemyActor_OnDefeat(object sender, EventArgs e) => audioSource.PlayOneShot(enemyDefeatSound);

        private void Bunny_OnDoDamage(object sender, EventArgs e) => audioSource.PlayOneShot(attackSound);

        private void Bunny_OnHealthChange(object sender, PointEventArgs e)
        {
            // FIXME: Called for each Bunny
            if (e.DeltaPoints >= 0)
                audioSource.PlayOneShot(healSound);
        }

        private void Bunny_OnSkillPointsChange(object sender, PointEventArgs e)
        {
            if (e.DeltaPoints >= 0)
                audioSource.PlayOneShot(healSound);
        }

        private void Enemy_OnDefeat(object sender, EventArgs e)
        {
            if (sender is Enemy)
            {
                Enemy enemy = sender as Enemy;
                enemy.OnDefeat -= Enemy_OnDefeat;
                enemy.OnDoDamage -= Enemy_OnDoDamage;
                enemy.OnHealthChange -= Enemy_OnHealthChange;
            }
        }

        private void Enemy_OnDoDamage(object sender, EventArgs e) => audioSource.PlayOneShot(attackSound);

        private void Enemy_OnHealthChange(object sender, PointEventArgs e)
        {
            // FIXME: Called for each Enemy
            if (e.DeltaPoints >= 0)
                audioSource.PlayOneShot(healSound);
        }
    }
}