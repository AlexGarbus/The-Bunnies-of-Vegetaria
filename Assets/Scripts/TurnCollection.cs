using System;
using System.Collections;
using System.Collections.Generic;

namespace TheBunniesOfVegetaria
{
    public class TurnCollection : ICollection, IEnumerable, IEnumerable<Turn>, IReadOnlyCollection<Turn>
    {
        private List<Turn> turns;

        public bool IsEmpty => turns.Count == 0;
        public bool IsSynchronized => ((ICollection)turns).IsSynchronized;
        public int Count => turns.Count;
        public object SyncRoot => ((ICollection)turns).SyncRoot;

        public void Clear() => turns.Clear();
        public void CopyTo(Array array, int index) => ((ICollection)turns).CopyTo(array, index);
        public IEnumerator GetEnumerator() => ((ICollection)turns).GetEnumerator();
        IEnumerator<Turn> IEnumerable<Turn>.GetEnumerator() => ((ICollection<Turn>)turns).GetEnumerator();

        public TurnCollection()
        {
            turns = new List<Turn>();
        }

        public TurnCollection(int capacity)
        {
            turns = new List<Turn>(capacity);
        }

        /// <summary>
        /// Insert a new turn into the list. The turn's index will be determined based on its user speed.
        /// </summary>
        /// <param name="turn">The turn to insert into the list.</param>
        public void Insert(Turn turn)
        {
            if (!IsEmpty)
            {
                for (int i = 0; i < turns.Count; i++)
                {
                    if (turn.user.Speed >= turns[i].user.Speed)
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
        /// Remove and return the first turn in the list.
        /// </summary>
        /// <returns>The first turn in the list.</returns>
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
        /// <param name="user">The fighter whose turns should be removed.</param>
        public void RemoveUserTurns(Fighter user)
        {
            turns.RemoveAll(turn => turn.user == user);
        }

        /// <summary>
        /// Remove all single-target turns in the list that target a specific fighter.
        /// </summary>
        /// <param name="target">The fighter whose targeted turns should be removed.</param>
        public void RemoveTargetTurns(Fighter target)
        {
            turns.RemoveAll(turn => turn.targets.Length == 1 && turn.targets[0] == target);
        }

        /// <summary>
        /// Remove all turns in the list that have at least one target.
        /// </summary>
        public void RemoveNonemptyTargetTurns()
        {
            turns.RemoveAll(turn => turn.targets.Length > 0);
        }

        /// <summary>
        /// Remove all turns in the list that have a bunny as their user.
        /// </summary>
        public void RemoveBunnyTurns()
        {
            turns.RemoveAll(turn => turn.user is Bunny);
        }

        /// <summary>
        /// Remove all turns in the list that have an enemy as their user.
        /// </summary>
        public void RemoveEnemyTurns()
        {
            turns.RemoveAll(turn => turn.user is Enemy);
        }
    }
}