using System;

namespace TheBunniesOfVegetaria
{
    public class BattleEventArgs : EventArgs
    {
        public Bunny[] Bunnies { get; }
        public Enemy[] Enemies { get; }
        public Bunny SelectedBunny { get; }
        public Turn CurrentTurn { get; }

        public BattleEventArgs(Bunny[] bunnies, Enemy[] enemies, Bunny selectedBunny, Turn currentTurn)
        {
            Bunnies = bunnies;
            Enemies = enemies;
            SelectedBunny = selectedBunny;
            CurrentTurn = currentTurn;
        }
    }
}