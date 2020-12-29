using System;

namespace TheBunniesOfVegetaria
{
    public class BattleEventArgs : EventArgs
    {
        public readonly Bunny[] bunnies;
        public readonly Enemy[] enemies;
        public readonly Bunny selectedBunny;
        public readonly Turn currentTurn;
        public readonly bool isBossWave;

        public BattleEventArgs(Bunny[] bunnies, Enemy[] enemies, Bunny selectedBunny, Turn currentTurn, bool isBossWave)
        {
            this.bunnies = bunnies;
            this.enemies = enemies;
            this.selectedBunny = selectedBunny;
            this.currentTurn = currentTurn;
            this.isBossWave = isBossWave;
        }
    }
}