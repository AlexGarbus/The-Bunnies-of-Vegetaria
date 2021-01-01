using UnityEngine;

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
            Target = Globals.FighterType.Enemy;
            Description = $"Random damage to all enemies.";
        }

        public override void Use(Bunny user, Fighter[] targets)
        {
            // Select a random damage multiplier and damage all targets
            float multiplier = Random.Range(minMultiplier, maxMultiplier);
            user.DoDamage(targets, multiplier);
        }
    }
}