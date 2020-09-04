using System;

namespace TheBunniesOfVegetaria
{
    public class Turn
    {
        public Fighter User { get; private set; }
        public Fighter[] Targets { get; private set; }
        public string Message { get; private set; }
        public Action TurnAction { get; private set; }

        /// <summary>
        /// Construct a new turn with no target.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <param name="turnAction"></param>
        public Turn(Fighter user, string message, Action turnAction)
        {
            User = user;
            Targets = new Fighter[0];
            Message = message;
            TurnAction = turnAction;
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
            User = user;
            Targets = new Fighter[] { target };
            Message = message;
            TurnAction = turnAction;
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
            User = user;
            Targets = targets;
            Message = message;
            TurnAction = turnAction;
        }
    }
}