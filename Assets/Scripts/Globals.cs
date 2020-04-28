using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public static class Globals
    {
        /// <summary>
        /// A battle scene area, used for selectively loading enemies and setting the background.
        /// </summary>
        public enum Area
        {
            LettuceFields = 1,
            CeleryWoods,
            BroccoliForest,
            BokChoyBluff,
            CarrotTop,
            Final
        }

        /// <summary>
        /// The type of a bunny in the player's party, used for determining base stats and skills.
        /// </summary>
        public enum BunnyType 
        { 
            Bunnight, 
            Bunnecromancer, 
            Bunnurse, 
            Bunneerdowell 
        }
    }
}