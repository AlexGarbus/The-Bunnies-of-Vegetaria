namespace TheBunniesOfVegetaria
{
    public class DamageAllSkill : Skill
    {
        private float multiplier;

        public DamageAllSkill(int cost, int minimumLevel, string name, float multiplier) : base(cost, minimumLevel, name)
        {
            this.multiplier = multiplier;
            Target = Globals.FighterType.Enemy;
            Description = $"{multiplier}x damage to all enemies";
        }

        public override void Use(Bunny user, Fighter[] targets)
        {
            // Damage all targets
            user.DoDamage(targets, multiplier);
        }
    }
}