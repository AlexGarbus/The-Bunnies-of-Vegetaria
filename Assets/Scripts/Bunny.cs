using UnityEngine;

// TODO: Move BunnyType to separate file/class/namespace
public enum BunnyType { Bunnight, Bunnecromancer, Bunnurse, Bunneerdowell };

public class Bunny : Fighter
{
    public int maxSkill;
    public BunnyType type;

    public int Level => level;
    public int Experience => experience;
    
    private int level;
    private int experience;

    public Bunny(BunnyType t, string n, int exp)
    {
        name = n;
        type = t;
        experience = exp;
        level = CalculateLevel();
        maxHealth = CalculateMaxHealth();
        maxSkill = CalculateMaxSkill();

        // TODO: Maybe load from JSON instead?
        switch(type)
        {
            case BunnyType.Bunnight:
                attack = 3;
                defense = 4;
                speed = 1;
                break;
            case BunnyType.Bunnecromancer:
                attack = 3;
                defense = 3;
                speed = 3;
                break;
            case BunnyType.Bunnurse:
                attack = 1;
                defense = 5;
                speed = 2;
                break;
            case BunnyType.Bunneerdowell:
                attack = 4;
                defense = 1;
                speed = 3;
                break;
        }
    }

    public void AddExperience(int value)
    {
        experience += value;
        if (experience > 1000)
            experience = 1000;
        level = CalculateLevel();
        maxHealth = CalculateMaxHealth();
        maxSkill = CalculateMaxSkill();
    }

    private int CalculateMaxHealth() => Mathf.FloorToInt(10 + 0.9f * CalculateLevel());

    private int CalculateLevel()
    {
        int n = 0;
        while (10 * (n * (n + 1) / 2) <= experience)
            n++;
        return n;
    }

    private int CalculateMaxSkill()
    {
        int level = CalculateLevel();
        return level % 2 == 0 ? level : level - 1;
    }
}