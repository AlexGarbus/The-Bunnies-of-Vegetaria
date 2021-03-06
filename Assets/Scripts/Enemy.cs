﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    [Serializable]
    public class Enemy : Fighter
    {
        public string spriteFileName, area;
        [SerializeField] protected bool singleAttack, multiAttack, singleHeal, multiHeal;

        public int ExperienceWorth => Mathf.RoundToInt(.4f * level - 2f);
        public override Globals.FighterType FighterType => Globals.FighterType.Bunny;

        private enum EnemyTurnType { SingleAttack, MultiAttack, SingleHeal, MultiHeal }

        private const int HEAL_AMOUNT = 10;

        public Enemy ShallowCopy() => (Enemy)MemberwiseClone();

        /// <summary>
        /// Generate a randomly-selected turn for this enemy actor.
        /// </summary>
        /// <param name="bunnies">All bunnies that this enemy can attack.</param>
        /// <param name="enemies">All enemies that this enemy can heal, including itself.</param>
        public Turn GetTurn(Bunny[] bunnies, Enemy[] enemies)
        {
            EnemyTurnType[] availableTurnTypes = GetAvailableTurnTypes(bunnies, enemies);

            if (availableTurnTypes.Length == 0)
            {
                Debug.LogError("Enemy has no available turns!");
                return null;
            }

            EnemyTurnType selectedTurn = availableTurnTypes[UnityEngine.Random.Range(0, availableTurnTypes.Length)];

            switch (selectedTurn)
            {
                case EnemyTurnType.SingleAttack:
                    Bunny bunny = bunnies[UnityEngine.Random.Range(0, bunnies.Length)];
                    return new Turn(this, bunny, $"{name} attacks {bunny.name}!", () => DoDamage(bunny));
                case EnemyTurnType.MultiAttack:
                    return new Turn(this, bunnies, $"{name} attacks the whole party!", () => DoDamage(bunnies));
                case EnemyTurnType.SingleHeal:
                    return new Turn(this, this, $"{name} healed itself!", () => Heal(HEAL_AMOUNT)
                    );
                case EnemyTurnType.MultiHeal:
                    return new Turn(this, enemies, $"{name} healed all enemies!", () =>
                        {
                            foreach (Enemy enemy in enemies)
                                enemy.Heal(HEAL_AMOUNT / 2);
                        }
                    );
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get an array of turn types that this enemy can use.
        /// </summary>
        /// <param name="bunnies">All bunnies that this enemy can attack.</param>
        /// <param name="enemies">All enemies that this enemy can heal, including itself.</param>
        /// <returns>An array of all turn types available to this enemy.</returns>
        private EnemyTurnType[] GetAvailableTurnTypes(Bunny[] bunnies, Enemy[] enemies)
        {
            List<EnemyTurnType> availableTurns = new List<EnemyTurnType>(4);

            if (singleAttack)
                availableTurns.Add(EnemyTurnType.SingleAttack);
            if (multiAttack && bunnies.Length > 1)
                availableTurns.Add(EnemyTurnType.MultiAttack);
            if (singleHeal && CurrentHealth < MaxHealth)
                availableTurns.Add(EnemyTurnType.SingleHeal);
            if (multiHeal && CurrentHealth < MaxHealth && enemies.Length > 1)
                availableTurns.Add(EnemyTurnType.MultiHeal);

            return availableTurns.ToArray();
        }
    }
}