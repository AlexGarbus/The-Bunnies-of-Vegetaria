using System;

namespace TheBunniesOfVegetaria
{
    public class PointEventArgs : EventArgs
    {
        public int PreviousPoints { get; }
        public int CurrentPoints { get; }
        public int DeltaPoints => CurrentPoints - PreviousPoints;

        public PointEventArgs(int previousPoints, int currentPoints)
        {
            PreviousPoints = previousPoints;
            CurrentPoints = currentPoints;
        }
    }
}