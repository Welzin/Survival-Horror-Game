using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        left = (KeyCode.Q, KeyCode.LeftArrow);
        right = (KeyCode.D, KeyCode.RightArrow);
        up = (KeyCode.Z, KeyCode.UpArrow);
        down = (KeyCode.S, KeyCode.DownArrow);
        lamp = (KeyCode.A, KeyCode.Mouse0);
        run = (KeyCode.LeftControl, KeyCode.LeftShift);

        DontDestroyOnLoad(gameObject);
    }
    public (KeyCode, KeyCode) LeftKey()
    {
        return left;
    }
    public (KeyCode, KeyCode) RightKey()
    {
        return right;
    }
    public (KeyCode, KeyCode) UpKey()
    {
        return up;
    }
    public (KeyCode, KeyCode) DownKey()
    {
        return down;
    }
    public (KeyCode, KeyCode) BindLamp()
    {
        return lamp;
    }
    public (KeyCode, KeyCode) Run()
    {
        return run;
    }

    private (KeyCode, KeyCode) left;
    private (KeyCode, KeyCode) right;
    private (KeyCode, KeyCode) up;
    private (KeyCode, KeyCode) down;
    private (KeyCode, KeyCode) lamp;
    private (KeyCode, KeyCode) run;
}
