using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public delegate void Delegate();

    public IActor User { get; private set; }
    public IActor Target { get; private set; }
    public string Message { get; private set; }
    public Action TurnAction { get; private set; }

    public Turn(IActor user, string message, Action turnAction)
    {
        User = user;
        Target = null;
        Message = message;
        TurnAction = turnAction;
    }

    public Turn(IActor user, IActor target, string message, Action turnAction)
    {
        User = user;
        Target = target;
        Message = message;
        TurnAction = turnAction;
    }
}
