using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    public GameObject goTo;
    public int intervalUntilNextAction;

    public Pattern(GameObject act, int interval, int movement)
    {
        goTo = act;
        intervalUntilNextAction = interval;
    }

    public Pattern() { }
}
