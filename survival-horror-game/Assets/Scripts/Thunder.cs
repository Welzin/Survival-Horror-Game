using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    private void Start()
    {
        _emiter = gameObject.AddComponent<SoundEmiter>();
        _emiter.SetNoiseEmited(NoiseType.Lightning);
        _loop = false;

        _allLightning = GetComponentsInChildren<Light>();

        foreach (Light light in _allLightning)
        {
            light.SetIntensity(0);
        }
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

        _emiter.PlayCustomClip(Resources.Load<AudioClip>("Effects/Thunder/tonnerre" + random), 200);
        _emiter.EmitSoundWave(200, 1, 4);
    }

    private IEnumerator NewLoopStrike(float intervalMin, float intervalMax)
    {
        while (_loop)
        {
            Strike();
            yield return new WaitForSeconds(Random.Range(intervalMin, intervalMax));
        }
    }

    public IEnumerator StartWink(float minFrequency, float maxFrequency, int numberOfWink, float minValue = 0f, float maxValue = 1f)
    {
        bool intensify = true;

        for (int i = 0; i <= numberOfWink; i++)
        {
            if (_allLightning.Length == 0)
                break;

            float actualIntensity = _allLightning[0].intensity;
            float newIntensity;

            if (intensify)
            {
                newIntensity = Random.Range(actualIntensity, maxValue);
            }
            else
            {
                newIntensity = Random.Range(minValue, actualIntensity);
            }

            foreach (Light light in _allLightning)
            {
                light.SetIntensity(newIntensity);
            }

            intensify = !intensify;
            yield return new WaitForSeconds(1f / Random.Range(minFrequency, maxFrequency));
        }

        foreach (Light light in _allLightning)
        {
            light.SetIntensity(0);
        }
    }

    private SoundEmiter _emiter;
    private bool _loop;

    private Light[] _allLightning;
}
