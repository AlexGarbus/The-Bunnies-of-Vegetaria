﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (IsEmpty)
        {
            turns.Add(turn);
        }
        else
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
}
