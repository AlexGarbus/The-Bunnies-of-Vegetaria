using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class Bunny : Fighter
    {
        public Globals.BunnyType Type { get; private set; }
        public int MaxSkillPoints { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public Skill[] Skills { get; private set; } = new Skill[3];

        // TODO: Construct from JSON
        public Bunny(Globals.BunnyType bunnyType, string name, int experience)
        {
            base.name = name;
            Type = bunnyType;
            Experience = experience;
            Level = CalculateLevel();
            MaxHealth = CalculateMaxHealth();
            MaxSkillPoints = CalculateMaxSkillPoints();

            switch(Type)
            {
                case Globals.BunnyType.Bunnight:
                    attack = 4;
                    defense = 4;
                    speed = 1;
                    Skills[0] = new DamageAllSkill(5, 5, "Slash", 0.5f);
                    Skills[1] = new DamageRandomSkill(10, 10, "Slash of Faith", 2);
                    Skills[2] = new DamageAllSkill(20, 20, "Wide Slash", 1);
                    break;
                case Globals.BunnyType.Bunnecromancer:
                    attack = 3;
                    defense = 3;
                    speed = 4;
                    Skills[0] = new RestoreSkillPointsAllSkill(5, 5, "Energize", 10);
                    Skills[1] = new DamageRandomSkill(10, 10, "Magic Missile", 2);
                    Skills[2] = new RestoreSkillPointsAllSkill(20, 20, "Synergize", 20);
                    break;
                case Globals.BunnyType.Bunnurse:
                    attack = 2;
                    defense = 5;
                    speed = 2;
                    Skills[0] = new HealAllSkill(5, 5, "Heal", 10);
                    Skills[1] = new ReviveAllSkill(10, 5, "Revive", 10);
                    Skills[2] = new ReviveAllSkill(20, 20, "Resurrect", 25);
                    break;
                case Globals.BunnyType.Bunneerdowell:
                    attack = 4;
                    defense = 2;
                    speed = 3;
                    Skills[0] = new DamageAllSkill(5, 5, "Fist Flurry", 0.5f);
                    Skills[1] = new RandomDamageAllSkill(10, 10, "Blind Fury", 0.25f, 2);
                    Skills[2] = new DamageAllSkill(20, 20, "Burning Rage", 1);
                    break;
            }
        }

        /// <summary>
        /// Add to the current experience and adjust related variables.
        /// </summary>
        /// <param name="value"></param>
        public void AddExperience(int value)
        {
            Experience += value;

            if (Experience > 1000)
                Experience = 1000;

            Level = CalculateLevel();
            MaxHealth = CalculateMaxHealth();
            MaxSkillPoints = CalculateMaxSkillPoints();
        }

        /// <summary>
        /// Get the skill stored at a specific index.
        /// </summary>
        /// <param name="skillIndex"></param>
        /// <returns>The skill stored at skill index.</returns>
        public Skill GetSkill(int skillIndex)
        {
            if (skillIndex >= Skills.Length)
                return null;
            else
                return Skills[skillIndex];
        }

        /// <summary>
        /// Get all skills that can currently be used.
        /// </summary>
        /// <returns>An array of all available skills.</returns>
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

        /// <summary>
        /// Get the string representations of all skills that can currently be used.
        /// </summary>
        /// <returns>An array of the string representations of all available skills.</returns>
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

        #region Stat Formulas

        private int CalculateMaxHealth()
        {
            return Mathf.FloorToInt(10 + 0.9f * CalculateLevel());
        }

        private int CalculateLevel()
        {
            int n = 0;
            while (10 * (n * (n + 1) / 2f) <= Experience)
                n++;
            return n;
        }

        private int CalculateMaxSkillPoints()
        {
            int level = CalculateLevel();
            return level / 5 * 10;
        }

        #endregion
    }
}