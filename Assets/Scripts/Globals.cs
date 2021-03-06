﻿namespace TheBunniesOfVegetaria
{
    public static class Globals
    {
        /// <summary>
        /// A battle scene area. Used for selectively loading enemies and setting the background.
        /// </summary>
        public enum Area
        {
            None,
            LettuceFields,
            CeleryWoods,
            BroccoliForest,
            BokChoyBluff,
            CarrotTop,
            Final1,
            Final2
        }

        /// <summary>
        /// The type of a bunny in the player's party. Used for determining base stats and skills.
        /// </summary>
        public enum BunnyType 
        { 
            Bunnight, 
            Bunnecromancer, 
            Bunnurse, 
            Bunneerdowell 
        }

        /// <summary>
        /// The type of a fighter.
        /// </summary>
        public enum FighterType
        {
            Bunny,
            Enemy
        }
    }
}