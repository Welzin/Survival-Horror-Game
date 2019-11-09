using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class Television : Light
{
    private void Start()
    {
        _emiter = gameObject.AddComponent<SoundEmiter>();
        radius = 10;
        base.Start();
    }

    private void Update()
    {
        TurnOn();
    }

    public void TurnOn()
    {
        if (!_isWinking)
        {
            StartCoroutine(StartWink(10, 15, int.MaxValue, 0.001f, 0.002f, true));
            StartCoroutine(ChangeColor());
            _emiter.PlayCustomClip(televisionShow);
        }
    }

    public void TurnOff()
    {
        _isWinking = false;
        _emiter.StopEffect();
    }

    private IEnumerator ChangeColor()
    {
        LightSprite light = GetComponent<LightSprite>();

        while (IsWinking())
        {
            light.Color = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), light.Color.a);
            yield return new WaitForSeconds(4);
        }
    }

    public AudioClip televisionShow;
    private SoundEmiter _emiter;
}
