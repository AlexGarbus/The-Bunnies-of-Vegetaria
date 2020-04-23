public interface IActor
{
    int CurrentHealth { get; }

    int Attack { get; }

    int Defense { get; }

    int Speed { get; }
    
    string FighterName { get; }

    Fighter FighterInfo { set; }

    void DoDamage(IActor target);

    void TakeDamage(int damage);

    void Die();
}
