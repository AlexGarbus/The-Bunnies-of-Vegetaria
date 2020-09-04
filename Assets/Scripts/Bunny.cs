using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class Bunny : Fighter
    {
        public EventHandler OnLevelUp;
        public EventHandler<PointEventArgs> OnSkillPointsChange;

        public override Globals.FighterType FighterType => Globals.FighterType.Bunny;
        public bool IsDefending { protected get; set; } = false;
        public int CurrentSkillPoints { get; private set; }
        public int MaxSkillPoints => Level / 5 * 10;
        public Globals.BunnyType Type { get; private set; }
        public Skill[] Skills { get; private set; } = new Skill[3];

        // TODO: Construct from JSON instead
        public Bunny(Globals.BunnyType bunnyType, string name, int experience)
        {
            base.name = name;
            Type = bunnyType;
            Experience = experience;
            Level = CalculateLevel();

            // Set stats and skills based on type
            switch(Type)
            {
                case Globals.BunnyType.Bunnight:
                    attack = 4;
                    defense = 4;
                    speed = 1;
                    Skills[0] = new DamageAllSkill(5, 5, "SLASH", 0.5f);
                    Skills[1] = new DamageRandomSkill(10, 10, "SLASH OF FAITH", 2);
                    Skills[2] = new DamageAllSkill(20, 20, "WIDE SLASH", 1);
                    break;
                case Globals.BunnyType.Bunnecromancer:
                    attack = 3;
                    defense = 3;
                    speed = 4;
                    Skills[0] = new RestoreSkillPointsAllSkill(5, 5, "ENERGIZE", 10);
                    Skills[1] = new DamageRandomSkill(10, 10, "MAGIC MISSILE", 2);
                    Skills[2] = new RestoreSkillPointsAllSkill(20, 20, "SYNERGIZE", 20);
                    break;
                case Globals.BunnyType.Bunnurse:
                    attack = 2;
                    defense = 5;
                    speed = 2;
                    Skills[0] = new HealAllSkill(5, 5, "HEAL", 10);
                    Skills[1] = new ReviveAllSkill(10, 10, "REVIVE", 10);
                    Skills[2] = new ReviveAllSkill(20, 20, "RESURRECT", 25);
                    break;
                case Globals.BunnyType.Bunneerdowell:
                    attack = 4;
                    defense = 2;
                    speed = 3;
                    Skills[0] = new DamageAllSkill(5, 5, "FIST FLURRY", 0.5f);
                    Skills[1] = new RandomDamageAllSkill(10, 10, "BLIND FURY", 0.25f, 2);
                    Skills[2] = new DamageAllSkill(20, 20, "BURNING RAGE", 1);
                    break;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            CurrentSkillPoints = MaxSkillPoints;
        }

        public override int CalculateDamage(Fighter target)
        {
            return Mathf.CeilToInt((attack * 5 + Level * (1 - attack * 5f / 100f)) * (1 - (target.Defense - 1) * 0.2f));
        }

        public override void TakeDamage(int damage)
        {
            if (IsDefending)
                damage = Mathf.CeilToInt(damage / 2f);

            base.TakeDamage(damage);
        }

        /// <summary>
        /// Reverse the effects of defeat and restore this actor's health.
        /// </summary>
        /// <param name="healthAmount">The amount of health that this actor should have upon revival.</param>
        public void Revive(int healthAmount = 10)
        {
            if (IsAlive)
                return;

            CurrentHealth = Mathf.Clamp(healthAmount, 0, MaxHealth);
            InvokeOnHealthChange(0);
        }

        /// <summary>
        /// Add to the current experience and adjust the level.
        /// </summary>
        /// <param name="experience">The amount of experience to add.</param>
        public void AddExperience(int experience)
        {
            Experience += experience;

            // Clamp experience
            if (Experience > MAX_EXPERIENCE)
                Experience = MAX_EXPERIENCE;

            // Set level
            int previousLevel = Level;
            Level = CalculateLevel();

            // Check for level up
            if (Level != previousLevel)
                OnLevelUp?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Use a skill.
        /// </summary>
        /// <param name="skillIndex">The index of the skill to use.</param>
        /// <param name="targets">The fighters that this skill should target.</param>
        public void UseSkill(int skillIndex, Fighter[] targets)
        {
            if (!CanUseSkill(skillIndex))
                return;

            Skill skill = Skills[skillIndex];
            CurrentSkillPoints -= skill.Cost;
            skill.Use(this, targets);
        }

        /// <summary>
        /// Get all skills that can currently be used.
        /// </summary>
        /// <returns>An array of all available skills.</returns>
        public Skill[] GetAvailableSkills()
        {
            List<Skill> availableSkills = new List<Skill>();
            
            // Check whether each skill is available
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

            // Check whether each skill is available
            foreach (Skill skill in Skills)
            {
                if (Level >= skill.MinimumLevel)
                    availableSkills.Add(skill.ToString());
            }

            return availableSkills.ToArray();
        }

        public bool CanUseSkill(int skillIndex)
        {
            Skill skill = Skills[skillIndex];
            return CurrentSkillPoints >= skill.Cost && Level >= skill.MinimumLevel;
        }

        public void RestoreSkillPoints(int skillAmount)
        {
            if (!IsAlive)
                return;

            int previousSkillPoints = CurrentSkillPoints;
            CurrentSkillPoints += skillAmount;

            // Clamp skill points
            if (CurrentSkillPoints > MaxSkillPoints)
                CurrentSkillPoints = MaxSkillPoints;

            PointEventArgs args = new PointEventArgs(previousSkillPoints, CurrentSkillPoints);
            OnSkillPointsChange?.Invoke(this, args);
        }
    }
}