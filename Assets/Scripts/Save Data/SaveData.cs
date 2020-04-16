[System.Serializable]
public class SaveData
{
    public static SaveData current;

    // Party
    public Bunny bunnight, bunnecromancer, bunnurse, bunneerdowell;

    // Progression
    public int areasUnlocked;
    public bool[] enemiesSeen;
    public bool[] achievements;
    public bool[] tutorials;

    // Play time
    public float hours, minutes, seconds;

    public SaveData()
    {
        bunnight = new Bunny(3, 4, 1, "Bunnight");
        bunnecromancer = new Bunny(3, 3, 3, "Bunnecromancer");
        bunnurse = new Bunny(1, 5, 2, "Bunnurse");
        bunneerdowell = new Bunny(4, 1, 3, "Bunneerdowell");
        areasUnlocked = 1;
        hours = 0;
        minutes = 0;
        seconds = 0;
    }
}
