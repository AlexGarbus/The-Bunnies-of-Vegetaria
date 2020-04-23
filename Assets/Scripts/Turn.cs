using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn
{
    public delegate void Delegate();

    public IActor user;
    public IActor target;
    public Action turnAction;

    public Turn(IActor u, IActor t, Action action)
    {
        user = u;
        target = t;
        turnAction = action;
    }
}
