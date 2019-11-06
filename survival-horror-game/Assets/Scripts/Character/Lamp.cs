using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : Light
{
    private void Awake()
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

    public void Reload()
    {
        _actualBattery = Mathf.Min(_actualBattery + batteryGainOnReload, maxBattery); 
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
                if (_actualBattery <= 0) { _active = false;  }
                mesh.enabled = _active;

            }
        }
    }

    public float actualBattery {  get { return _actualBattery; } }

    public float maxBattery = 100f;
    public float consommationBySec = 10f;
    public float batteryGainOnReload;

    private bool _active;
    private float _actualBattery;
}
