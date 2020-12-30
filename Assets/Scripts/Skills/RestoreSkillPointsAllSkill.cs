namespace TheBunniesOfVegetaria
{
    public class RestoreSkillPointsAllSkill : Skill
    {
        private int skillAmount;

        public RestoreSkillPointsAllSkill(int cost, int minimumLevel, string name, int skillAmount) : base(cost, minimumLevel, name)
        {
            this.skillAmount = skillAmount;
            Target = Globals.FighterType.Bunny;
            Description = $"Party SP +{skillAmount}";
        }

        public override void Use(Bunny user, Fighter[] targets)
        {
            // Restore each target's skill points
            foreach (Fighter target in targets)
            {
                if (target is Bunny && target != user)
                {
                    Bunny targetBunny = (Bunny)target;
                    targetBunny.RestoreSkillPoints(skillAmount);
                }
            }
        }
    }
}