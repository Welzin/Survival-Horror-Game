using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        left = KeyCode.Q;
        right = KeyCode.D;
        up = KeyCode.Z;
        down = KeyCode.S;

        DontDestroyOnLoad(gameObject);
    }
    public KeyCode LeftKey()
    {
        return left;
    }
    public KeyCode RightKey()
    {
        return right;
    }
    public KeyCode UpKey()
    {
        return up;
    }
    public KeyCode DownKey()
    {
        return down;
    }

    private KeyCode left;
    private KeyCode right;
    private KeyCode up;
    private KeyCode down;
}
