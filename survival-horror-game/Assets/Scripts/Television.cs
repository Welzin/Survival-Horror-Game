using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D;

public class Television : Light
{
    protected void Start()
    {
        base.Start();
        radius = 5;
        _emiter = gameObject.AddComponent<SoundEmiter>();
        _teleOn = false;

        // This method is called after 1 second because when we add an emiter, the emiter's start method isn't called instantly
        // TurnOn() method need to called emiter.PlayCustomClip() and for this, emiter's start method should be called
        Invoke("TurnOn", 1);
    }

    private void OnEnable()
    {
        if (_emiter != null && _teleOn)
            TurnOn();
    }

    public void TurnOn()
    {
        if (!IsWinking())
        {
            Debug.Log("turnon");
            StartCoroutine(StartWink(10, 15, int.MaxValue, 0.001f, 0.002f, true));
            StartCoroutine(ChangeColor());
            _emiter.PlayCustomClip(televisionShow, 10);
            _teleOn = true;
        }
    }

    public void TurnOff()
    {
        Debug.Log("turnoff");
        StopWink();
        _emiter.StopEffect();
        _teleOn = false;
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
    bool _teleOn;
}
