﻿using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class RandomDamageAllSkill : Skill
    {
        private float minMultiplier;
        private float maxMultiplier;

        public RandomDamageAllSkill(int cost, int minimumLevel, string name, float min, float max) : base(cost, minimumLevel, name)
        {
            minMultiplier = min;
            maxMultiplier = max;
            Target = TargetType.Enemy;
            Description = $"Random damage to all enemies.";
        }

        public override void Use(BunnyActor user, IActor[] targets)
        {
            float multiplier = Random.Range(minMultiplier, maxMultiplier);
            user.DoDamage(targets, multiplier);
        }
    }
}