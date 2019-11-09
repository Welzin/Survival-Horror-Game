using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : Light
{
    private void Start()
    {
        _emiter = gameObject.AddComponent<SoundEmiter>();
        _loop = false;

        if (loopOnStart)
            StartLoopStrike(intervalMinimum, intervalMaximum);
    }

    public void Strike()
    {
        StartCoroutine(NewStrike());
    }

    public void StartLoopStrike(float intervalMin, float intervalMax)
    {
        _loop = true;
        StartCoroutine(NewLoopStrike(intervalMin, intervalMax));
    }

    public void StopLoopStrike()
    {
        _loop = false;
    }

    private IEnumerator NewStrike()
    {
        int random = Random.Range(1, 6);
        StartCoroutine(StartWink(10, 15, 25));
        yield return new WaitForSeconds(Random.Range(1, 3));

        _emiter.PlayCustomClip(Resources.Load<AudioClip>("Effects/Thunder/tonnerre" + random));
    }

    private IEnumerator NewLoopStrike(float intervalMin, float intervalMax)
    {
        while (_loop)
        {
            Strike();
            yield return new WaitForSeconds(Random.Range(intervalMin, intervalMax));
        }
    }

    private SoundEmiter _emiter;
    private bool _loop;

    public bool loopOnStart;
    public float intervalMinimum;
    public float intervalMaximum;
}
