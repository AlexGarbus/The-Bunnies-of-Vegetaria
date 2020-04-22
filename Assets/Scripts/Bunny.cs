using UnityEngine;

// TODO: Move BunnyType to separate file/class/namespace
public enum BunnyType { Bunnight, Bunnecromancer, Bunnurse, Bunneerdowell };

public class Bunny
{
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public int MaxHealth { get; private set; }
    public int MaxSkill { get; private set; }
    public int Experience { get; private set; }
    public int Level { get; private set; }
    public string Name { get; private set; }
    public BunnyType Type { get; private set; }

    public int CalculateMaxHealth() => Mathf.FloorToInt(10 + 0.9f * CalculateLevel());

    public int CalculateLevel()
    {
        int n = 0;
        while (10 * (n * (n + 1) / 2) <= Experience)
            n++;
        return n;
    }

    public int CalculateMaxSkill()
    {
        int level = CalculateLevel();
        return level % 2 == 0 ? level : level - 1;
    }

    public Bunny(BunnyType t, string n, int exp)
    {
        Name = n;
        Type = t;
        Experience = exp;
        Level = CalculateLevel();
        MaxHealth = CalculateMaxHealth();

        // TODO: Maybe load from JSON instead?
        switch(Type)
        {
            case BunnyType.Bunnight:
                Attack = 3;
                Defense = 4;
                Speed = 1;
                break;
            case BunnyType.Bunnecromancer:
                Attack = 3;
                Defense = 3;
                Speed = 3;
                break;
            case BunnyType.Bunnurse:
                Attack = 1;
                Defense = 5;
                Speed = 2;
                break;
            case BunnyType.Bunneerdowell:
                Attack = 4;
                Defense = 1;
                Speed = 3;
                break;
        }
    }
}
