using System;

namespace TheBunniesOfVegetaria
{
    public class Turn
    {
        public readonly Fighter user;
        public readonly Fighter[] targets;
        public readonly string message;
        public readonly Action turnAction;

        /// <summary>
        /// Construct a new turn with no target.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="turnAction"></param>
        public Turn(Fighter user, string message, Action turnAction)
        {
            this.user = user;
            targets = new Fighter[0];
            this.message = message;
            this.turnAction = turnAction;
        }

        /// <summary>
        /// Construct a new turn with a single target.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="target"></param>
        /// <param name="message"></param>
        /// <param name="turnAction"></param>
        public Turn(Fighter user, Fighter target, string message, Action turnAction)
        {
            this.user = user;
            targets = new Fighter[] { target };
            this.message = message;
            this.turnAction = turnAction;
        }

        /// <summary>
        /// Construct a new turn with multiple targets.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="targets"></param>
        /// <param name="message"></param>
        /// <param name="turnAction"></param>
        public Turn(Fighter user, Fighter[] targets, string message, Action turnAction)
        {
            this.user = user;
            this.targets = targets;
            this.message = message;
            this.turnAction = turnAction;
        }
    }
}