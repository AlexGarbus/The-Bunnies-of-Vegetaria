using System;

namespace TheBunniesOfVegetaria
{
    public class BattleEventArgs : EventArgs
    {
        public readonly Bunny[] bunnies;
        public readonly Enemy[] enemies;
        public readonly Bunny selectedBunny;
        public readonly Turn currentTurn;
        public readonly bool isFinalWave;

        public BattleEventArgs(Bunny[] bunnies, Enemy[] enemies, Bunny selectedBunny, Turn currentTurn, bool isFinalWave)
        {
            this.bunnies = bunnies;
            this.enemies = enemies;
            this.selectedBunny = selectedBunny;
            this.currentTurn = currentTurn;
            this.isFinalWave = isFinalWave;
        }
    }
}