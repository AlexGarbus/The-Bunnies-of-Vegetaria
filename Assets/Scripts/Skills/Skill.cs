namespace TheBunniesOfVegetaria
{
    public abstract class Skill
    {
        public int Cost { get; protected set; }
        public int MinimumLevel { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public Globals.FighterType Target { get; protected set; }

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

        /// <summary>
        /// Perform this skill.
        /// </summary>
        /// <param name="user">The bunny that will use this skill.</param>
        /// <param name="targets">The targets that will be affected by this skill.</param>
        public abstract void Use(Bunny user, Fighter[] targets);
    }
}