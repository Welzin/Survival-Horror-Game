using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private void Start()
    {
        Active = false;
    }

    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            if (GetComponent<MeshRenderer>())
            {
                MeshRenderer mesh = GetComponent<MeshRenderer>();
                _active = value;
                mesh.enabled = _active;
            }
        }
    }

    private bool _active;
}
