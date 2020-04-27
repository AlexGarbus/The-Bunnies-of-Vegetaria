using System.Collections;
using UnityEngine;

public interface IActor
{
    bool IsAlive { get; }

    int CurrentHealth { get; }

    int Attack { get; }

    int Defense { get; }

    int Speed { get; }

    int Experience { get; }
    
    string FighterName { get; }

    Vector2 StartPosition { get; }

    BattleEffect Effect { get; }

    BattleManager Manager { set; }

    Fighter FighterInfo { set; }

    int CalculateDamage(IActor target);

    void DoDamage(IActor target);

    void DoDamage(IActor[] targets);

    void TakeDamage(int damage);

    void Die();

    void Heal(int healAmount);

    IEnumerator TakeStep();
}