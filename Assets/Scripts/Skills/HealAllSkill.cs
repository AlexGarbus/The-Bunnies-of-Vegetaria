namespace TheBunniesOfVegetaria
{
    public class HealAllSkill : Skill
    {
        private int healAmount;

        public HealAllSkill(int cost, int minimumLevel, string name, int healAmount) : base(cost, minimumLevel, name)
        {
            this.healAmount = healAmount;
            Target = TargetType.Bunny;
            Description = $"Party HP +{healAmount}";
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