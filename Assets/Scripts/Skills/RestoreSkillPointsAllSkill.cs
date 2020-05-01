namespace TheBunniesOfVegetaria
{
    public class RestoreSkillPointsAllSkill : Skill
    {
        private int skillAmount;

        public RestoreSkillPointsAllSkill(int cost, int minimumLevel, string name, int skillAmount) : base(cost, minimumLevel, name)
        {
            this.skillAmount = skillAmount;
            Target = TargetType.Bunny;
            Description = $"Party SP +{skillAmount}";
        }

        public override void Use(BunnyActor user, IActor[] targets)
        {
            foreach (IActor target in targets)
            {
                if (target is BunnyActor && (BunnyActor)target != user)
                {
                    BunnyActor targetBunny = (BunnyActor)target;
                    targetBunny.RestoreSkillPoints(skillAmount);
                }
            }
        }
    }
}