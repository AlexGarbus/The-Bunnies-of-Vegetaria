using System;
using System.Collections;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public interface IActor
    {
        event Action<IActor> OnHealthZero;
        event Action<IActor> OnDefeat;
        bool IsAlive { get; }
        int Attack { get; }
        int Defense { get; }
        int Speed { get; }
        int Experience { get; }
        int CurrentHealth { get; }
        string FighterName { get; }
        Vector2 StartPosition { get; }
        BattleEffect Effect { get; }
        Fighter FighterData { set; }
        int CalculateDamage(IActor target);
        void DoDamage(IActor target, float multiplier = 1);
        void DoDamage(IActor[] targets, float multiplier = 0.5f);
        void TakeDamage(int damage);
        void Defeat();
        void Heal(int healAmount);
        IEnumerator TakeStep();
    }
}