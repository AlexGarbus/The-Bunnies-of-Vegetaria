using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class Bunny : Fighter
    {
        public int maxSkill;
        public Globals.BunnyType type;

        public int Level { get; private set; }
        public int Experience { get; private set; }
        public Skill[] Skills { get; private set; } = new Skill[3];

        public Bunny(Globals.BunnyType t, string n, int exp)
        {
            name = n;
            type = t;
            Experience = exp;
            Level = CalculateLevel();
            maxHealth = CalculateMaxHealth();
            maxSkill = CalculateMaxSkill();

            switch(type)
            {
                case Globals.BunnyType.Bunnight:
                    attack = 3;
                    defense = 4;
                    speed = 1;
                    Skills[0] = new DamageAll(5, 5, "Slash", 0.5f);
                    Skills[1] = new DamageRandom(10, 10, "Slash of Faith", 2);
                    Skills[2] = new DamageAll(20, 20, "Wide Slash", 1);
                    break;
                case Globals.BunnyType.Bunnecromancer:
                    attack = 3;
                    defense = 3;
                    speed = 3;
                    Skills[0] = new RestoreSkillAll(5, 5, "Energize", 10);
                    Skills[1] = new DamageRandom(10, 10, "Magic Missile", 2);
                    Skills[2] = new RestoreSkillAll(20, 20, "Synergize", 20);
                    break;
                case Globals.BunnyType.Bunnurse:
                    attack = 1;
                    defense = 5;
                    speed = 2;
                    Skills[0] = new HealAll(5, 5, "Heal", 10);
                    Skills[1] = new HealAll(10, 10, "Super Heal", 20);
                    Skills[2] = new ReviveAll(20, 20, "Revive");
                    break;
                case Globals.BunnyType.Bunneerdowell:
                    attack = 4;
                    defense = 1;
                    speed = 3;
                    Skills[0] = new DamageAll(5, 5, "Fist Flurry", 0.5f);
                    Skills[1] = new RandomDamageAll(10, 10, "Blind Fury", 0.25f, 2);
                    Skills[2] = new DamageAll(20, 20, "Burning Rage", 1);
                    break;
            }
        }

        public void AddExperience(int value)
        {
            Experience += value;
            if (Experience > 1000)
                Experience = 1000;
            Level = CalculateLevel();
            maxHealth = CalculateMaxHealth();
            maxSkill = CalculateMaxSkill();
        }

        public Skill GetSkill(int skillIndex)
        {
            return Skills[skillIndex];
        }

        public Skill[] GetAvailableSkills()
        {
            List<Skill> availableSkills = new List<Skill>();
            
            foreach(Skill skill in Skills)
            {
                if (Level >= skill.MinimumLevel)
                    availableSkills.Add(skill);
            }

            return availableSkills.ToArray();
        }

        public string[] GetAvailableSkillStrings()
        {
            List<string> availableSkills = new List<string>();

            foreach (Skill skill in Skills)
            {
                if (Level >= skill.MinimumLevel)
                    availableSkills.Add(skill.ToString());
            }

            return availableSkills.ToArray();
        }

        private int CalculateMaxHealth() => Mathf.FloorToInt(10 + 0.9f * CalculateLevel());

        private int CalculateLevel()
        {
            int n = 0;
            while (10 * (n * (n + 1) / 2) <= Experience)
                n++;
            return n;
        }

        private int CalculateMaxSkill()
        {
            int level = CalculateLevel();
            return level / 5 * 5;
        }
    }
}