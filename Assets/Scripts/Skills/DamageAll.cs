﻿using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class DamageAll : Skill
    {
        private float multiplier;

        public DamageAll(int cost, int minimumLevel, string name, float multiplier) : base(cost, minimumLevel, name)
        {
            this.multiplier = multiplier;
            Target = TargetType.Enemy;
            Description = $"{multiplier}x damage to all enemies";
        }

        public override void Use(BunnyActor user, IActor[] targets)
        {
            user.DoDamage(targets, multiplier);
        }
    }
}