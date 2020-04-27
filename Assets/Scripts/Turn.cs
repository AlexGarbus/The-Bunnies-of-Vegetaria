using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public IActor User { get; private set; }
    public IActor[] Targets { get; private set; }
    public string Message { get; private set; }
    public Action TurnAction { get; private set; }

    /// <summary>
    /// Construct a new turn with no target.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="message"></param>
    /// <param name="turnAction"></param>
    public Turn(IActor user, string message, Action turnAction)
    {
        User = user;
        Targets = new IActor[0];
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
    public Turn(IActor user, IActor target, string message, Action turnAction)
    {
        User = user;
        Targets = new IActor[] { target };
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
    public Turn(IActor user, IActor[] targets, string message, Action turnAction)
    {
        User = user;
        Targets = targets;
        Message = message;
        TurnAction = turnAction;
    }
}
