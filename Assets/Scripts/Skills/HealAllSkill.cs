namespace TheBunniesOfVegetaria
{
    public class HealAllSkill : Skill
    {
        private int healAmount;

        public HealAllSkill(int cost, int minimumLevel, string name, int healAmount) : base(cost, minimumLevel, name)
        {
            this.healAmount = healAmount;
            Target = Globals.FighterType.Bunny;
            Description = $"Party HP +{healAmount}";
        }

        public override void Use(Bunny user, Fighter[] targets)
        {
            foreach (Fighter fighter in targets)
                fighter.Heal(healAmount);
        }
    }
}