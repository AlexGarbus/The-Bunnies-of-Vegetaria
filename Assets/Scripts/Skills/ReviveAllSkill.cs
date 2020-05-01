namespace TheBunniesOfVegetaria
{
    public class ReviveAllSkill : Skill
    {
        private int healthAmount;

        public ReviveAllSkill(int cost, int minimumLevel, string name, int healthAmount) : base(cost, minimumLevel, name)
        {
            this.healthAmount = healthAmount;
            Target = TargetType.Bunny;
            Description = $"Revive fallen allies with {healthAmount} HP";
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