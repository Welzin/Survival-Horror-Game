using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private void Start()
    {
        _actualBattery = maxBattery;
        Active = false;
    }

    private void Update()
    {
        if (_active)
        {
            _actualBattery -= consommationBySec * Time.deltaTime;
            if (_actualBattery <= 0)
            {
                _actualBattery = 0;
                Active = false;
            }
        }
    }

    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            if (_actualBattery > 0 && GetComponent<MeshRenderer>())
            {
                MeshRenderer mesh = GetComponent<MeshRenderer>();
                _active = value;
                mesh.enabled = _active;
            }
        }
    }

    public float maxBattery = 100f;
    public float consommationBySec = 10f;

    private bool _active;
    private float _actualBattery;
}
