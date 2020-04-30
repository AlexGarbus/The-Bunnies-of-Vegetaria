namespace TheBunniesOfVegetaria
{
    [System.Serializable]
    public class Enemy : Fighter
    {
        public string spriteFileName, area;
        public bool singleAttack, multiAttack, singleHeal, multiHeal;

        public Enemy()
        {
            MaxHealth = 100;
        }
    }
}