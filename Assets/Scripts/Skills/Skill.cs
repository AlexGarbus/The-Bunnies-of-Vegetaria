namespace TheBunniesOfVegetaria
{
    public abstract class Skill
    {
        public int Cost { get; protected set; }
        public int MinimumLevel { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public enum TargetType { Bunny, Enemy }

        protected TargetType type;

        public Skill(int cost, int minimumLevel, string name)
        {
            Cost = cost;
            MinimumLevel = minimumLevel;
            Name = name;
        }

        public abstract void Use(IActor user, IActor[] targets);
    }
}