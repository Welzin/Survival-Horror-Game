using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    public Actions action;
    public int intervalUntilNextAction;
    public int movementAmplitude;

    public Pattern(Actions act, int interval, int movement)
    {
        action = act;
        intervalUntilNextAction = interval;
        movementAmplitude = movement;
    }

    public Pattern() { }
}
