using System;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public abstract class Fighter
    {
        public string name;

        [SerializeField] protected int attack;
        [SerializeField] protected int defense;
        [SerializeField] protected int speed;
        [SerializeField] protected int level;

        public event EventHandler OnDoDamage, OnDefeat;
        public event EventHandler<PointEventArgs> OnHealthChange;

        public abstract Globals.FighterType FighterType { get; }
        public bool IsAlive => CurrentHealth > 0;
        public int Experience { get; protected set; }
        public int Level => level;
        public int CurrentHealth { get; protected set; }
        public int MaxHealth => Mathf.FloorToInt(10 + 0.9f * Level);
        public int Attack => attack;
        public int Defense => defense;
        public int Speed => speed;

        protected const int MAX_EXPERIENCE = 5050;

        public virtual void Initialize()
        {
            CurrentHealth = MaxHealth;
        }
        
        public int CalculateDamage(Fighter target)
        {
            int baseDamage = attack * 5;
            float bonusDamage = Level * (baseDamage / 100f);
            float defenseMultiplier = 1 - (target.Defense - 1) * 0.2f;  // TODO: Defense multiplier should be affected by target level.
            return Mathf.CeilToInt((baseDamage + bonusDamage) * defenseMultiplier);
        }

        /// <summary>
        /// Do damage to a single target fighter.
        /// </summary>
        /// <param name="target">The target fighter.</param>
        /// <param name="multiplier">The amount to multiply the damage by.</param>
        public void DoDamage(Fighter target, float multiplier = 1)
        {
            DoDamage(new Fighter[] { target }, multiplier);
        }

        /// <summary>
        /// Do damage to multiple target fighters.
        /// </summary>
        /// <param name="targets">The target fighters to damage.</param>
        /// <param name="multiplier">The amount to multiply the damage by.</param>
        public void DoDamage(Fighter[] targets, float multiplier = 0.5f)
        {
            if (!IsAlive)
                return;

            OnDoDamage?.Invoke(this, EventArgs.Empty);

            foreach (Fighter target in targets)
            {
                int damage = Mathf.CeilToInt(CalculateDamage(target) * multiplier);
                target.TakeDamage(damage);
            }
        }

        public virtual void TakeDamage(int damage)
        {
            if (!IsAlive)
                return;

            int previousHealth = CurrentHealth;
            CurrentHealth -= damage;
            PointEventArgs args = new PointEventArgs(previousHealth, CurrentHealth);
            OnHealthChange?.Invoke(this, args);

            // Check for defeat
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnDefeat?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Add health to this fighter.
        /// </summary>
        /// <param name="healthAmount">The amount of health to add.</param>
        public void Heal(int healthAmount)
        {
            if (!IsAlive)
                return;

            int previousHealth = CurrentHealth;
            CurrentHealth += healthAmount;

            // Clamp health
            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;

            PointEventArgs args = new PointEventArgs(previousHealth, CurrentHealth);
            OnHealthChange?.Invoke(this, args);
        }

        protected int CalculateLevel()
        {
            int n = 0;
            while ((n * (n + 1) / 2f) <= Experience)
                n++;
            return n;
        }

        protected void InvokeOnDoDamage() => OnDoDamage?.Invoke(this, EventArgs.Empty);

        protected void InvokeOnDefeat() => OnDefeat?.Invoke(this, EventArgs.Empty);

        protected void InvokeOnHealthChange(int previousHealth)
        {
            PointEventArgs args = new PointEventArgs(previousHealth, CurrentHealth);
            OnHealthChange?.Invoke(this, args);
        }
    }
}