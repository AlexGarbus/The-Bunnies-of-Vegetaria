using System;

namespace TheBunniesOfVegetaria
{
    public class PointEventArgs : EventArgs
    {
        public readonly int previousPoints;
        public readonly int currentPoints;

        public int DeltaPoints => currentPoints - previousPoints;

        public PointEventArgs(int previousPoints, int currentPoints)
        {
            this.previousPoints = previousPoints;
            this.currentPoints = currentPoints;
        }
    }
}