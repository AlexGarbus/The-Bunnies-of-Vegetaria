namespace TheBunniesOfVegetaria
{
    public class ReviveAllSkill : Skill
    {
        private int healthAmount;

        public ReviveAllSkill(int cost, int minimumLevel, string name, int healthAmount) : base(cost, minimumLevel, name)
        {
            this.healthAmount = healthAmount;
            Target = Globals.FighterType.Bunny;
            Description = $"Revive fallen allies with {healthAmount} HP";
        }

        public override void Use(Bunny user, Fighter[] targets)
        {
            // Revive all defeated targets
            foreach (Fighter target in targets)
            {
                if (target is Bunny && !target.IsAlive)
                {
                    Bunny bunny = (Bunny)target;
                    bunny.Revive(healthAmount);
                }
            }
        }
    }
}