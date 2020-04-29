using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class ReviveAll : Skill
    {
        private int healthAmount;

        public ReviveAll(int cost, int minimumLevel, string name, int healthAmount) : base(cost, minimumLevel, name)
        {
            this.healthAmount = healthAmount;
            Target = TargetType.Bunny;
            Description = $"Revive defeated party members";
        }

        public override void Use(BunnyActor user, IActor[] targets)
        {
            foreach (IActor target in targets)
            {
                if (!target.IsAlive && target is BunnyActor)
                {
                    BunnyActor targetBunny = (BunnyActor)target;
                    targetBunny.Revive(healthAmount);
                }
            }
        }
    }
}