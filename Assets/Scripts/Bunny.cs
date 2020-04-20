using UnityEngine;

// TODO: Convert to MonoBehaviour
[System.Serializable]
public class Bunny
{
    public int attack, defense, speed, experience;
    public string name;

    public int GetMaxHealth() => Mathf.FloorToInt(10 + 0.9f * GetLevel());

    public int GetLevel()
    {
        int n = 0;
        while (10 * (n * (n + 1) / 2) <= experience)
            n++;
        return n;
    }

    public int GetMaxSkillPoints()
    {
        int level = GetLevel();
        return level % 2 == 0 ? level : level - 1;
    }

    public Bunny(int a, int d, int s, string n)
    {
        attack = a;
        defense = d;
        speed = s;
        name = n;
        experience = 0;
    }
}
