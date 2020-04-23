public interface IActor
{
    int CurrentHealth { get; }

    int Attack { get; }

    int Defense { get; }

    int Speed { get; }
    
    //int AttackDamage { get; }

    string FighterName { get; }

    Fighter FighterInfo { set; }


    void TakeDamage(int damage);

    void Die();
}
