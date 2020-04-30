namespace TheBunniesOfVegetaria
{
    [System.Serializable]
    public class SaveData
    {
        public static SaveData current;

        // Party
        public int bunnightExp, bunnecromancerExp, bunnurseExp, bunneerdowellExp;
        public string bunnightName, bunnecromancerName, bunnurseName, bunneerdowellName;

        // Progression
        public int areasUnlocked;

        // Play time
        public float hours, minutes, seconds;

        // Unused progression variables
        // Will be implemented in future versions
        public bool[] enemiesSeen;
        public bool[] achievements;
        public bool[] tutorials;

        public SaveData()
        {
            bunnightName = "Bunnight";
            bunnecromancerName = "Bunnecromancer";
            bunnurseName = "Bunnurse";
            bunneerdowellName = "Bunne'er-do-well";
            areasUnlocked = 1;
        }
    }
}
