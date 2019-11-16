using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private void Awake()
    {
        _actualBattery = maxBattery;
        Active = false;
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

    public float actualBattery {  get { return _actualBattery; } set { _actualBattery = value; } }

    public float maxBattery = 100f;
    public float consommationBySec = 10f;
    public float batteryGainOnReload;
    public float radius;

    private bool _active;
    private float _actualBattery;
}
