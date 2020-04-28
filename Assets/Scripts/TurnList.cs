using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class TurnList
    {
        private List<Turn> turns;

        public bool IsEmpty => turns.Count == 0;

        public TurnList()
        {
            turns = new List<Turn>();
        }

        public TurnList(int capacity)
        {
            turns = new List<Turn>(capacity);
        }

        /// <summary>
        /// Insert a new turn into the list. The turn's order will be determined based on its user speed.
        /// </summary>
        /// <param name="turn">The turn to insert into the list.</param>
        public void Insert(Turn turn)
        {
            if (!IsEmpty)
            {
                for (int i = 0; i < turns.Count; i++)
                {
                    if (turn.User.Speed >= turns[i].User.Speed)
                    {
                        turns.Insert(i, turn);
                        return;
                    }
                }
            }

            turns.Add(turn);
        }

        /// <summary>
        /// Insert a new turn at the beginning of the list.
        /// </summary>
        /// <param name="turn">The turn to insert into the list.</param>
        public void Push(Turn turn)
        {
            turns.Insert(0, turn);
        }

        /// <summary>
        /// Add a turn to the end of the list.
        /// </summary>
        /// <param name="turn">The turn to add to the end of the list.</param>
        public void Append(Turn turn)
        {
            turns.Add(turn);
        }

        /// <summary>
        /// Remove and return the first turn in the list. This turn will have the fastest user in the list.
        /// </summary>
        /// <returns>The turn with the fastest user in the list.</returns>
        public Turn Pop()
        {
            if (IsEmpty)
                return null;

            Turn turn = turns[0];
            turns.RemoveAt(0);
            return turn;
        }

        /// <summary>
        /// Remove all turns in the list that belong to a specific user.
        /// </summary>
        /// <param name="user">The actor whose turns should be removed.</param>
        public void RemoveUserTurns(IActor user)
        {
            turns.RemoveAll(turn => turn.User == user);
        }

        /// <summary>
        /// Remove all turns in the list that only target one specific actor.
        /// </summary>
        /// <param name="user">The actor whose targeted turns should be removed.</param>
        public void RemoveTargetTurns(IActor user)
        {
            int removed = 0;
            foreach (Turn turn in turns)
                if (turn.Targets.Length == 1 && turn.Targets[0] == user)
                    removed++;
            turns.RemoveAll(turn => turn.Targets.Length == 1 && turn.Targets[0] == user);
        }

        /// <summary>
        /// Remove all turns in the list that have a bunny as their user.
        /// </summary>
        public void RemoveBunnyTurns()
        {
            turns.RemoveAll(turn => turn.User is BunnyActor);
        }

        /// <summary>
        /// Remove all turns in the list that have an enemy as their user.
        /// </summary>
        public void RemoveEnemyTurns()
        {
            turns.RemoveAll(turn => turn.User is EnemyActor);
        }

        /// <summary>
        /// Clear the turn list.
        /// </summary>
        public void Clear()
        {
            turns.Clear();
        }
    }
}