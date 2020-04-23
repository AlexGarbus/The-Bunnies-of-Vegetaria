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

    public Turn(IActor u, IActor t, string m, Action action)
    {
        User = u;
        Target = t;
        Message = m;
        TurnAction = action;
    }
}
