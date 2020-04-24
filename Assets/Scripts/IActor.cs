using System.Collections;
using UnityEngine;

public interface IActor
{
    int CurrentHealth { get; }

    int Attack { get; }

    int Defense { get; }

    int Speed { get; }
    
    string FighterName { get; }

    Vector2 StartPosition { get; }

    Fighter FighterInfo { set; }

    BattleEffect Effect { get; }

    void DoDamage(IActor target);

    void TakeDamage(int damage);

    void Die();

    IEnumerator TakeStep();
}