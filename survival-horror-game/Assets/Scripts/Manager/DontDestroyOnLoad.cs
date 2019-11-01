using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Controls
{
    Left,
    Right,
    Up,
    Down,
    Lamp,
    Run,
    Interact,
    Reload,
    HugTeddy,
}
public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        _allKeys = new Dictionary<Controls, (KeyCode, KeyCode)>();

        AssignKey(Controls.Left, KeyCode.Q, KeyCode.LeftArrow);
        AssignKey(Controls.Right, KeyCode.D, KeyCode.RightArrow);
        AssignKey(Controls.Up, KeyCode.Z, KeyCode.UpArrow);
        AssignKey(Controls.Down, KeyCode.S, KeyCode.DownArrow);
        AssignKey(Controls.Lamp, KeyCode.A, KeyCode.Mouse0);
        AssignKey(Controls.Run, KeyCode.LeftControl, KeyCode.None);
        AssignKey(Controls.Interact, KeyCode.E, KeyCode.Mouse1);
        AssignKey(Controls.Reload, KeyCode.R, KeyCode.None);
        AssignKey(Controls.HugTeddy, KeyCode.F, KeyCode.None);

        DontDestroyOnLoad(gameObject);
    }

    private void AssignKey(Controls key, KeyCode keyOne, KeyCode keyTwo)
    {
        _allKeys[key] = (keyOne, keyTwo);
    }

    public Dictionary<Controls, (KeyCode, KeyCode)> AllKeys()
    {
        return _allKeys;
    }

    public (KeyCode, KeyCode) GetKey(Controls control)
    {
        return _allKeys[control];
    }

    public void SetKey(Controls control, KeyCode keyOne, KeyCode keyTwo)
    {
        AssignKey(control, keyOne, keyTwo);
    }

    private Dictionary<Controls, (KeyCode, KeyCode)> _allKeys;
}
