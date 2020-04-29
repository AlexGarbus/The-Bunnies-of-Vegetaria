using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class HealAll : Skill
    {
        private int healAmount;

        public HealAll(int cost, int minimumLevel, string name, int healthAmount) : base(cost, minimumLevel, name)
        {
            this.healAmount = healthAmount;
            Target = TargetType.Bunny;
            Description = $"Party HP +{healthAmount}";
        }

        public override void Use(BunnyActor user, IActor[] targets)
        {
            foreach (IActor target in targets)
            {
                target.Heal(healAmount);
            }
        }
    }
}