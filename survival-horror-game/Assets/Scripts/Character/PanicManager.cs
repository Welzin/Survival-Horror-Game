using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicManager : MonoBehaviour
{
    void Start()
    {
        _player = FindObjectOfType<PlayerManager>();
        _panic = false;
    }
    
    void Update()
    {

    }

    public bool IsPanicking()
    {
        return _panic;
    }

    public IEnumerator StartPanicking()
    {
        _panic = true;
        if (!_player.lamp.Active && _player.lamp.actualBattery > 0f)
        {
            _player.ToggleLamp();
        }

        yield return null;
        _panic = false;
    }

    private PlayerManager _player;
    bool _panic;
}
