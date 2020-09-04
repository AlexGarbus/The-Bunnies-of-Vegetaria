using UnityEngine;

namespace TheBunniesOfVegetaria
{ 
    public class DamageRandomSkill : Skill
    {
        private float multiplier;

        public DamageRandomSkill(int cost, int minimumLevel, string name, float multiplier) : base(cost, minimumLevel, name) 
        {
            this.multiplier = multiplier;
            Target = Globals.FighterType.Enemy;
            Description = $"{multiplier}x damage to random enemy";
        }

        public override void Use(Bunny user, Fighter[] targets)
        {
            // Select a random enemy to damage
            int targetIndex = Random.Range(0, targets.Length);
            user.DoDamage(targets[targetIndex], multiplier);
        }
    }
}