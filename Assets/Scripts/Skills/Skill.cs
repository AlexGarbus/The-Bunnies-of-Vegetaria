namespace TheBunniesOfVegetaria
{
    public abstract class Skill
    {
        public int Cost { get; protected set; }
        public int MinimumLevel { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public enum TargetType { Bunny, Enemy }

        public TargetType Target { get; protected set; }

        public Skill(int cost, int minimumLevel, string name)
        {
            Cost = cost;
            MinimumLevel = minimumLevel;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name} ({Cost} SP): {Description}";
        }

        public abstract void Use(BunnyActor user, IActor[] targets);
    }
}